namespace BookStoreAPIs.DTOs.Response
{
    public class GetBookResponse
    {
        public IEnumerable<BookResponse> Books { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
