namespace RESTfulAPI.Services
{
    public class VehicleService
    {
        private String _Patent;
        private String _Brand;
        private String _Model;
        private String _Type;
        private int _Year;
        //private String _Driver;

        public VehicleService()
        {

        }

        public String Patent
        {
            get { return _Patent; }
            set { _Patent = value; }
        }
        public String Brand
        {
            get { return _Brand; }
            set { _Brand = value; }
        }

        public String Model
        {
            get { return _Model; }
            set { _Model = value; }
        }

        public String Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public int Year
        {
            get { return _Year; }
            set { _Year = value; }
        }

        //public String Driver
        //{
        //    get { return _Driver; }
        //    set { _Driver = value; }
        //}

    }
}
