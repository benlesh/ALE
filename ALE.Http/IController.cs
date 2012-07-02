namespace ALE.Http
{
	public interface IController
	{
		IRequest Request { get; set; }

		IResponse Response { get; set; }

		IContext Context { get; set; }
	}
}