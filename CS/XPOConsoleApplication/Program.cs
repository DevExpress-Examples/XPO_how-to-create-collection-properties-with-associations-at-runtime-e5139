using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;

namespace XPOConsoleApplication {
    class Program {
        static void Main(string[] args) {
            XPDictionary dict = new ReflectionDictionary();
            Type[] types = new Type[] { typeof(Supplier), typeof(Product), typeof(Group) };
            dict.CollectClassInfos(types);

            XPClassInfo ciSupplier = dict.GetClassInfo(typeof(Supplier));
            XPClassInfo ciProduct = dict.GetClassInfo(typeof(Product));
            XPClassInfo ciGroup = dict.GetClassInfo(typeof(Group));
            //A one-to-many association
            XPMemberInfo miSupplier = ciProduct.CreateMember("Supplier", typeof(Supplier), new AssociationAttribute("SupplierProducts"));
            XPMemberInfo miProducts = ciSupplier.CreateMember("Products", typeof(XPCollection), true, new AssociationAttribute("SupplierProducts", typeof(Product)));
            //A many-to-many association
            XPMemberInfo miGroups = ciProduct.CreateMember("Groups", typeof(XPCollection), true, new AssociationAttribute("GroupsItems", typeof(Group)));
            XPMemberInfo miItems = ciGroup.CreateMember("Items", typeof(XPCollection), true, new AssociationAttribute("GroupsItems", typeof(Product)));
            
            //Creating a data layer
            IDataStore provider = new InMemoryDataStore();
            //IDataStore provider = XpoDefault.GetConnectionProvider(MSSqlConnectionProvider.GetConnectionString("localhost", ""), AutoCreateOption.DatabaseAndSchema);
            XpoDefault.DataLayer = new SimpleDataLayer(dict, provider);
            XpoDefault.Session = null;

            //Creating sample data
            using (UnitOfWork session = new UnitOfWork()) {
                session.ClearDatabase();
                session.UpdateSchema(types);
                session.CreateObjectTypeRecords(types);

                Supplier s1 = new Supplier(session) { CompanyName = "Acme", Address = "XY/101" };
                Supplier s2 = new Supplier(session) { CompanyName = "Zorq", Address = "Moonbase" };
                Product p1 = new Product(session) { Name = "Anvil", Quantity = 1 };
                miSupplier.SetValue(p1, s1);
                Product p2 = new Product(session) { Name = "Cow", Quantity = 80 };
                ((XPCollection)miProducts.GetValue(s1)).Add(p2);
                Product p3 = new Product(session) { Name = "Saucer", Quantity = 3 };
                ciProduct.GetMember("Supplier").SetValue(p3, s2);
                Group g1 = new Group(session) { Title = "Flying" };
                ((XPCollection)miItems.GetValue(g1)).Add(p2);
                ((XPCollection)miGroups.GetValue(p3)).Add(g1);
                session.CommitChanges();
            }

            //Some tests
            using (UnitOfWork session = new UnitOfWork()) {
                Group g1 = session.FindObject<Group>(CriteriaOperator.Parse("Title='Flying'"));
                XPCollection groupItems = (XPCollection)g1.GetMemberValue("Items");
                System.Diagnostics.Debug.Assert(groupItems.Count == 2);
                Supplier s1 = session.FindObject<Supplier>(CriteriaOperator.Parse("CompanyName='Acme'"));
                XPCollection acmeProducts = (XPCollection)s1.GetMemberValue("Products");
                System.Diagnostics.Debug.Assert(acmeProducts.Count == 2);
                Product p3 = session.FindObject<Product>(CriteriaOperator.Parse("Name='Saucer'"));
                System.Diagnostics.Debug.Assert(((Supplier)miSupplier.GetValue(p3)).CompanyName == "Zorq");
            }
        }
    }

    public class Supplier : XPObject {
        public Supplier(Session session) : base(session) { }
        public string CompanyName {
            get { return GetPropertyValue<string>("CompanyName"); }
            set { SetPropertyValue<string>("CompanyName", value); }
        }
        public string Address {
            get { return GetPropertyValue<string>("Address"); }
            set { SetPropertyValue<string>("Address", value); }
        }
    }

    public class Product : XPObject {
        public Product(Session session) : base(session) { }
        public string Name {
            get { return GetPropertyValue<string>("Name"); }
            set { SetPropertyValue<string>("Name", value); }
        }
        public int Quantity {
            get { return GetPropertyValue<int>("Quantity"); }
            set { SetPropertyValue<int>("Quantity", value); }
        }
    }

    public class Group : XPObject {
        public Group(Session session) : base(session) { }
        public string Title {
            get { return GetPropertyValue<string>("Title"); }
            set { SetPropertyValue<string>("Title", value); }
        }
    }
}
