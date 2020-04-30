using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
namespace rkbc.web.constant
{
    public class Country
    {
        public string code { set; get; }
        public string code3 { set; get; }
        public string name { get; set; }
        public string longName { set; get; }
        public string koreanName { set; get; }
        public string number { set; get; }
        public string dialCode { get; set; }
    }
    
    public static class Constants
    {
        public static readonly string AllCats = "AllCats";
        public static readonly string categories = "categories";
        public static readonly string Dash = "-";
        public static readonly string Description = "Description";
        public static readonly string Head = "Head";
        public static readonly string next = "next";
        public static readonly string page = "page";
        public static readonly string Preload = "Preload";
        public static readonly string prev = "prev";
        public static readonly string ReturnUrl = "ReturnUrl";
        public static readonly string Scripts = "Scripts";
        public static readonly string slug = "slug";
        public static readonly string Space = " ";
        public static readonly string Title = "Title";
        public static readonly string TotalPostCount = "TotalPostCount";
        public static readonly string ViewOption = "ViewOption";
        
        public static List<Country> getCountryList()
        {
            var lst = new List<Country>();
            lst.Add(new Country {code = "KH", code3 = "KHM", koreanName = "캄보디아", name = "Cambodia", longName = "Cambodia", number = "116", dialCode = "+855" });
            lst.Add(new Country {code= "CN", code3= "CHN", koreanName = "중국,홍콩", name ="China", longName= "China", number= "156", dialCode="+86"});
            lst.Add(new Country {code= "MX", code3= "MEX", koreanName = "멕시코", name ="Mexico", longName= "Mexico", number= "484", dialCode="+52"});
            lst.Add(new Country {code= "BF", code3= "BFA", koreanName = "브키나파소", name ="Burkina Faso", longName= "Burkina Faso", number= "854", dialCode="+226"});
            lst.Add(new Country {code= "PH", code3= "PHL", koreanName = "필리핀", name ="Philippines", longName= "Philippines (the)", number= "608", dialCode="+63"});
            lst.Add(new Country {code= "KP", code3= "PRK", koreanName = "북한", name ="North Korea", longName= "Korea (the Democratic People's Republic of)", number= "408", dialCode="+850"});
            lst.Add(new Country {code= "KR", code3= "KOR", koreanName = "한국", name ="South Korea", longName= "Korea (the Democratic People's Republic of)", number= "410", dialCode="+82"});
            lst.Add(new Country {code= "US", code3= "USA", koreanName = "미국", name="United States of America", longName= "United States of America (the)", number= "840", dialCode="+1"});
            lst.Add(new Country {code= "DO", code3= "DOM", koreanName = "도미니카", name="Dominican Republic", longName= "Dominican Republic (the)", number= "214", dialCode="+1 849"});
            lst.Add(new Country {code= "VN", code3= "VNM", koreanName = "베트남", name = "VietNam", longName= "VietNam", number= "704", dialCode="+84"});
            return lst;
        }

        //[SuppressMessage(
        //    "Design",
        //    "CA1034:Nested types should not be visible",
        //    Justification = "Constant classes are nested for easy intellisense.")]
        //public static class Config
        //{
        //    public static class Blog
        //    {
        //        public static readonly string Name = "blog:name";
        //    }

        //    public static class User
        //    {
        //        public static readonly string Password = "user:password";
        //        public static readonly string Salt = "user:salt";
        //        public static readonly string UserName = "user:username";
        //    }
        //}
    }
}
