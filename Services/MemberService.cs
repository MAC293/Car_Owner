namespace RESTfulAPI.Services
{
    public class MemberService
    {

        private String _ID;
        private String _Username;
        private String _Password;

        public MemberService()
        {

        }
        public String ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public String Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

    }

}
