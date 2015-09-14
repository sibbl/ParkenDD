using ParkenDD.Models;

namespace ParkenDD.Messages
{
    public class ShowSearchResultOnMapMessage
    {
        public AddressSearchSuggestionItem Result { get; }

        public ShowSearchResultOnMapMessage(AddressSearchSuggestionItem result)
        {
            Result = result;
        }
    }
}
