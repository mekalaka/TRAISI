namespace TRAISI.SDK.Library.ResponseTypes
{
	public interface IRangeResponse : IResponseType
	{
		double MinValue { get; set; }

		double MaxValue { get; set; }
	}
}