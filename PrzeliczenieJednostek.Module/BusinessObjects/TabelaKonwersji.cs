using Common.Module.Module.BusinessObjects;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzeliczenieJednostek.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class TabelaKonwersji : XPObject
    {
        public TabelaKonwersji(Session session) : base(session)
        { }


        JednostkaLicznosci jednostkaLicznosci;
        JednostkaWagi jednostkaWagi;

        public JednostkaWagi JednostkaWagi
        {
            get => jednostkaWagi;
            set => SetPropertyValue(nameof(JednostkaWagi), ref jednostkaWagi, value);
        }

        
        public JednostkaLicznosci JednostkaLicznosci
        {
            get => jednostkaLicznosci;
            set => SetPropertyValue(nameof(JednostkaLicznosci), ref jednostkaLicznosci, value);
        }
    }
}
