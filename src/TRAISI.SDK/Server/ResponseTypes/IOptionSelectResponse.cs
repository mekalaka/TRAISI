namespace TRAISI.SDK.Library.ResponseTypes
{
    public interface IOptionSelectResponse : IResponseType
    {
        string Value { get; set; }


        string Code { get; set; }
    }
}