using System;

namespace WPTestPJ.Common.BusinessLogics
{
    public class SQLServerProxy : IDisposable
    {
        public WPlocalEntities _wplocalEntities = null;

        public SQLServerProxy()
        {
            this._wplocalEntities = new WPlocalEntities();
        }

        public void Dispose()
        {
            this._wplocalEntities.Dispose();
        }
    }
}