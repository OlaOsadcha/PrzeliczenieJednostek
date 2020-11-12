using Common.Module.Module.BusinessObjects;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Drawing.Design;
using System.Linq;

namespace PrzeliczenieJednostek.Module.BusinessObjects
{
    [XafDisplayName("Kalkulacja podaży")]
    [DefaultClassOptions]
    [NavigationItem("Kalkulacja")]
    public class KalkulacjaPodazy : BaseObject
    {
        private JednostkaMiary jednostka;
        ////private PelnaKalkulacja kalkulacja;
        private Parametr parametr;
       
        private decimal podazEnteralna;
        private decimal podazParenteralna;
        private decimal podazPlanowana;
        private decimal podazPlanowanaKg;
        private JednostkaMiary jednostkaPrzeliczeniowa;      

        public KalkulacjaPodazy(Session session) : base(session)
        {
        }
      
        [Index(5)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        ///[ModelDefault(nameof(IModelCommonMemberViewItem.AllowEdit), "false")]
        [ImmediatePostData]
        public JednostkaMiary Jednostka
        {
            get
            {
                return this.jednostka;
            }
            set
            {
                bool modified = SetPropertyValue(nameof(Jednostka), ref jednostka, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    WyliczPodazParenteralna();
                }
            }
        }

        ////[Association("Kalkulacja-KalkulacjePodazy")]
        ////public PelnaKalkulacja Kalkulacja
        ////{
        ////    get => kalkulacja;
        ////    set => SetPropertyValue(nameof(Kalkulacja), ref kalkulacja, value);
        ////}

        [Index(0)]
        public Parametr Parametr
        {
            get => parametr;
            set => SetPropertyValue(nameof(Parametr), ref parametr, value);
        }

        [XafDisplayName("Podaż entralna")]
        [Index(2)]
        [ModelDefault("AllowEdit", "False")]
        [ImmediatePostData]
        public decimal PodazEnteralna
        {
            get => podazEnteralna;
            set
            {
                bool modified = SetPropertyValue(nameof(PodazEnteralna), ref podazEnteralna, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    WyliczPodazParenteralna();
                }
            }
        }

        [Index(3)]
        [XafDisplayName("Podaż parenteralna")]
        [ModelDefault("AllowEdit", "False")]
        [ImmediatePostData]
        public decimal PodazParenteralna
        {
            get => podazParenteralna;
            set => SetPropertyValue(nameof(PodazParenteralna), ref podazParenteralna, value);
        }

        [Index(1)]
        [XafDisplayName("Plan/kg")]
        [ModelDefault(nameof(IModelCommonMemberViewItem.EditMask), "N4")]
        [ModelDefault(nameof(IModelCommonMemberViewItem.DisplayFormat), "{0:N4}")]
        [ImmediatePostData]
        public decimal PodazPlanowanaKg
        {
            get => podazPlanowanaKg;
            set
            {
                bool modified = SetPropertyValue(nameof(PodazPlanowanaKg), ref podazPlanowanaKg, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    PrzeliczPodazPlanowana();
                }
            }
        }

        [Index(1)]
        [XafDisplayName("Plan")]
        [ImmediatePostData]
        public decimal PodazPlanowana
        {
            get => podazPlanowana;
            set
            {
                bool modified = SetPropertyValue(nameof(PodazPlanowana), ref podazPlanowana, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    ////if (Kalkulacja?.MasaCialaWKg > 0)
                    ////{
                    ////    podazPlanowanaKg = PodazPlanowana / Kalkulacja.MasaCialaWKg;
                    ////}
                    WyliczPodazParenteralna();
                }
            }
        }

        [XafDisplayName("Par.[mmol]")]
        [VisibleInListView(false)]
        public decimal WartoscWmmol
        {
            get
            {
                if (Jednostka is JednostkaMiary)
                {                   
                    decimal wartosc = PodazParenteralna / Jednostka.Przelicznik;
                    return wartosc;
                }
                if (Jednostka is JednostkaWagi && Parametr?.LiczbaMolowa > 0)
                {
                    return PodazParenteralna / Parametr.LiczbaMolowa * 1000 / Jednostka.Przelicznik;
                }
                return 0;
            }
        }

        [XafDisplayName("Par.[g]")]
        [VisibleInListView(false)]
        public decimal WartoscWgram
        {
            get
            {
                if (Jednostka is JednostkaMiary)
                {
                    decimal wartosc = PodazParenteralna / Jednostka.Przelicznik;                  
                    return wartosc;
                }
                if (Jednostka is JednostkaLicznosci && Parametr?.LiczbaMolowa > 0)
                {
                    return PodazParenteralna / Jednostka.Przelicznik * Parametr.LiczbaMolowa / 1000;
                }
                return 0;
            }
        }

        [VisibleInListView(false)]
        [XafDisplayName("Wartość przeliczona")]
        public decimal WartoscPrzeliczona
        {
            get
            {
                if (this.Parametr != null)
                {
                    bool liczycWgMoli = Parametr.JednostkaBazowa is JednostkaLicznosci;
                    if (liczycWgMoli)
                    {
                        return WartoscWmmol;// StezenieJonow / 1000 * Parametr.Jednostka.Przelicznik;// 1000 bo przeliczamy z mikromoli na milimole
                    }
                    else
                    {
                        return WartoscWgram; // /Jednostka.Przelicznik;
                    }
                }
                return 0;
            }
        }

        [XafDisplayName("Jednostka przeliczeniowa")]
        [VisibleInListView(false)]
        public JednostkaMiary JednostkaPrzeliczeniowa
        {
            get
            {
                if (this.Parametr != null)
                {
                    jednostkaPrzeliczeniowa = Parametr.JednostkaBazowa;
                }
                return jednostkaPrzeliczeniowa;
            }
        }

        internal void PrzeliczPodazPlanowana()
        {
           // podazPlanowana = PodazPlanowanaKg * Kalkulacja.MasaCialaWKg;
            WyliczPodazParenteralna();
        }       

        private void WyliczPodazParenteralna()
        {
            PodazParenteralna = PodazPlanowana - PodazEnteralna;
            ////if (Kalkulacja != null)
            ////{
            ////    Kalkulacja.UstawPlanowanaPodazParenteralna(Parametr, PodazParenteralna, Jednostka);
            ////}         

        }       
    }
}