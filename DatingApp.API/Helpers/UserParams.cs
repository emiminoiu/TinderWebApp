namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber {get;set;} = 1;
        private int pageSize = 10;

        public string Gender {get;set;}
        public int UserId {get;set;}
        public int minAge {get;set;} = 18;
        public int maxAge {get;set;} = 99;

        public string OrderBy {get;set;}

        public bool Likers {get;set;} = false;
        public bool Likees {get;set;} = false;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value) > MaxPageSize ? value = MaxPageSize : value; }
        }
        
    }
}