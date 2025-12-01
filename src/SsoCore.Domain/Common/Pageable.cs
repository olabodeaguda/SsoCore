namespace SsoCore.Domain.Common
{
    public class Pageable<T>
    {
        public int TotalItems { get; set;  }
        public int PageSize { get; set;  }
        public int CurrentPage { get; set;  }
        public int TotalPages { get; set;  }
        public List<T> Data { get; set;  }
        public static Pageable<T> Empty
        {
            get
            {
                return new Pageable<T>(new List<T>(), 0, 1, 10);
            }
        }

        public Pageable(List<T> data, int totalItems, int pageNumber, int pageSize)
        {
            TotalItems = totalItems;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            Data = data;
        }

        public static Pageable<T> Create(List<T> data, int totalItems, int pageNumber, int pageSize)
        {
            return new Pageable<T>(data, totalItems, pageNumber, pageSize);
        }
    }
}
