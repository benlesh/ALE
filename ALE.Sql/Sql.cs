using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ALE.Sql
{
	public class Sql : IDisposable
	{
		protected readonly SqlConnection Connection;
		protected readonly string ConnectionString;

		private Sql(string connectionString)
		{
			ConnectionString = connectionString;
			Connection = new SqlConnection(ConnectionString);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (Connection != null)
			{
				Connection.Dispose();
			}
		}

		#endregion

		public static Sql Create(string connectionString)
		{
			return new Sql(connectionString);
		}

		private SqlParameter[] ConvertObjectToSqlParams(object args)
		{
			var properties = args.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetField);
			var parameters = new SqlParameter[properties.Length];
			for (var i = 0; i < properties.Length; i++)
			{
				var property = properties[i];
				parameters[i] = new SqlParameter(property.Name, property.GetValue(args, null));
			}
			return parameters;
		}

		public SqlCommand CreateCommand(string commandText, object args = null, CommandType commandType = CommandType.Text)
		{
			var cmd = new SqlCommand(commandText, Connection);
			cmd.CommandType = commandType;
			if (args != null)
			{
				cmd.Parameters.AddRange(ConvertObjectToSqlParams(args));
			}
			return cmd;
		}

		public Sql ExecuteReader(string commandText, Action<Exception, SqlDataReader> callback)
		{
			return ExecuteReader(commandText, null, callback);
		}

		public Sql ExecuteReader(string commandText, object args, Action<Exception, SqlDataReader> callback)
		{
			return ExecuteReader(commandText, args, CommandType.Text, callback);
		}

		public Sql ExecuteReader(string commandText, object args, CommandType cmdType, Action<Exception, SqlDataReader> callback)
		{
			var cmd = CreateCommand(commandText, args, cmdType);
			return ExecuteReader(cmd, callback);
		}

		public Sql ExecuteReader(SqlCommand cmd, Action<Exception, SqlDataReader> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			cmd.Connection.Open();
			var state = new ExecuteReaderState(cmd, callback);
			cmd.BeginExecuteReader(EndExecuteReader, state);
			return this;
		}

		private void EndExecuteReader(IAsyncResult result)
		{
			var state = (ExecuteReaderState)result.AsyncState;
			try
			{
				var reader = state.Command.EndExecuteReader(result);
				EventLoop.Pend(() => state.Callback(null, reader));
			} catch (Exception ex)
			{
				EventLoop.Pend(() => state.Callback(ex, null));
			}
		}

		public Sql ExecuteNonQuery(string commandText, Action<Exception, int> callback = null)
		{
			return this.ExecuteNonQuery(commandText, null, callback);
		}

		public Sql ExecuteNonQuery(string commandText, object args, Action<Exception, int> callback = null)
		{
			return ExecuteNonQuery(commandText, args, CommandType.Text);
		}

		public Sql ExecuteNonQuery(string commandText, object args, CommandType commandType, Action<Exception, int> callback = null)
		{
			var cmd = CreateCommand(commandText, args, commandType);
			return ExecuteNonQuery(cmd, callback);
		}

		public Sql ExecuteNonQuery(SqlCommand cmd, Action<Exception,int> callback = null)
		{
			cmd.Connection.Open();
			var state = new ExecuteNonQueryState(cmd, callback);
			cmd.BeginExecuteNonQuery(ExecuteNonQueryCallback, state);
			return this;
		}

		public void ExecuteNonQueryCallback(IAsyncResult result)
		{
			var state = (ExecuteNonQueryState) result.AsyncState;
			try
			{
				var recordsAffected = state.Command.EndExecuteNonQuery(result);
				EventLoop.Pend(() => state.Callback(null, recordsAffected));
			} catch (Exception ex)
			{
				EventLoop.Pend(() => state.Callback(ex, -1));
			}
		}
	}

	public class ExecuteNonQueryState
	{
		public readonly SqlCommand Command;
		public readonly Action<Exception, int> Callback;

		public ExecuteNonQueryState(SqlCommand cmd, Action<Exception, int> callback)
		{
			Command = cmd;
			Callback = callback;
		}
	}

	public class ExecuteReaderState
	{
		public readonly SqlCommand Command;
		public readonly Action<Exception, SqlDataReader> Callback;

		public ExecuteReaderState(SqlCommand cmd, Action<Exception, SqlDataReader> callback)
		{
			Command = cmd;
			Callback = callback;
		}
	}
}