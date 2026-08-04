namespace LondonEstate.Utils.Types
{
    public class Pagination
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int CurrentPage { get; private set; }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;


    }
}
