Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo.Metadata

Namespace XPOConsoleApplication
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			Dim dict As XPDictionary = New ReflectionDictionary()
			Dim types() As Type = { GetType(Supplier), GetType(Product), GetType(Group) }
			dict.CollectClassInfos(types)

			Dim ciSupplier As XPClassInfo = dict.GetClassInfo(GetType(Supplier))
			Dim ciProduct As XPClassInfo = dict.GetClassInfo(GetType(Product))
			Dim ciGroup As XPClassInfo = dict.GetClassInfo(GetType(Group))
			'A one-to-many association
			Dim miSupplier As XPMemberInfo = ciProduct.CreateMember("Supplier", GetType(Supplier), New AssociationAttribute("SupplierProducts"))
			Dim miProducts As XPMemberInfo = ciSupplier.CreateMember("Products", GetType(XPCollection), True, New AssociationAttribute("SupplierProducts", GetType(Product)))
			'A many-to-many association
			Dim miGroups As XPMemberInfo = ciProduct.CreateMember("Groups", GetType(XPCollection), True, New AssociationAttribute("GroupsItems", GetType(Group)))
			Dim miItems As XPMemberInfo = ciGroup.CreateMember("Items", GetType(XPCollection), True, New AssociationAttribute("GroupsItems", GetType(Product)))

			'Creating a data layer
			Dim provider As IDataStore = New InMemoryDataStore()
			'IDataStore provider = XpoDefault.GetConnectionProvider(MSSqlConnectionProvider.GetConnectionString("localhost", ""), AutoCreateOption.DatabaseAndSchema);
			XpoDefault.DataLayer = New SimpleDataLayer(dict, provider)
			XpoDefault.Session = Nothing

			'Creating sample data
			Using session As New UnitOfWork()
				session.ClearDatabase()
				session.UpdateSchema(types)
				session.CreateObjectTypeRecords(types)

				Dim s1 As New Supplier(session) With {
					.CompanyName = "Acme",
					.Address = "XY/101"
				}
				Dim s2 As New Supplier(session) With {
					.CompanyName = "Zorq",
					.Address = "Moonbase"
				}
				Dim p1 As New Product(session) With {
					.Name = "Anvil",
					.Quantity = 1
				}
				miSupplier.SetValue(p1, s1)
				Dim p2 As New Product(session) With {
					.Name = "Cow",
					.Quantity = 80
				}
				CType(miProducts.GetValue(s1), XPCollection).Add(p2)
				Dim p3 As New Product(session) With {
					.Name = "Saucer",
					.Quantity = 3
				}
				ciProduct.GetMember("Supplier").SetValue(p3, s2)
				Dim g1 As New Group(session) With {.Title = "Flying"}
				CType(miItems.GetValue(g1), XPCollection).Add(p2)
				CType(miGroups.GetValue(p3), XPCollection).Add(g1)
				session.CommitChanges()
			End Using

			'Some tests
			Using session As New UnitOfWork()
				Dim g1 As Group = session.FindObject(Of Group)(CriteriaOperator.Parse("Title='Flying'"))
				Dim groupItems As XPCollection = CType(g1.GetMemberValue("Items"), XPCollection)
				System.Diagnostics.Debug.Assert(groupItems.Count = 2)
				Dim s1 As Supplier = session.FindObject(Of Supplier)(CriteriaOperator.Parse("CompanyName='Acme'"))
				Dim acmeProducts As XPCollection = CType(s1.GetMemberValue("Products"), XPCollection)
				System.Diagnostics.Debug.Assert(acmeProducts.Count = 2)
				Dim p3 As Product = session.FindObject(Of Product)(CriteriaOperator.Parse("Name='Saucer'"))
				System.Diagnostics.Debug.Assert(CType(miSupplier.GetValue(p3), Supplier).CompanyName = "Zorq")
			End Using
		End Sub
	End Class

	Public Class Supplier
		Inherits XPObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Public Property CompanyName() As String
			Get
				Return GetPropertyValue(Of String)("CompanyName")
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("CompanyName", value)
			End Set
		End Property
		Public Property Address() As String
			Get
				Return GetPropertyValue(Of String)("Address")
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("Address", value)
			End Set
		End Property
	End Class

	Public Class Product
		Inherits XPObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Public Property Name() As String
			Get
				Return GetPropertyValue(Of String)("Name")
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("Name", value)
			End Set
		End Property
		Public Property Quantity() As Integer
			Get
				Return GetPropertyValue(Of Integer)("Quantity")
			End Get
			Set(ByVal value As Integer)
				SetPropertyValue(Of Integer)("Quantity", value)
			End Set
		End Property
	End Class

	Public Class Group
		Inherits XPObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Public Property Title() As String
			Get
				Return GetPropertyValue(Of String)("Title")
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("Title", value)
			End Set
		End Property
	End Class
End Namespace
