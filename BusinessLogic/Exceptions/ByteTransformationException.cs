namespace ProjectHellsParadise.BusinessLogic.Exceptions;

public class ByteTransformationException : Exception
{
    public  ByteTransformationException(string message)
        : base(message)
    {}
    
    public   ByteTransformationException(string message, Exception inner)
        : base(message, inner)
    {}
}