namespace GeneticSearch.Services
{
    public interface IRleService
    {
        string Decode(string encoded);
        string Encode(string plain);
    }
}