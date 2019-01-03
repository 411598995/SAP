using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace ACHR
{

  public  class NopServices
    {
        DataServices dsNOP;
        DataServices dsSAP;
        string constrNOP = "";
        string constrSAP = "";
        public SAPbobsCOM.Company oDiCompany;
        public DIClass sboApi;

        bool isDIConnected = false;

        public NopServices(string cnNop, string cnSAP, DIClass pSboApi)
        {
            dsNOP = new DataServices(cnNop);
            dsSAP = new DataServices(cnSAP);
            sboApi = pSboApi;
        }
        public int syncRoleWithCG()
        {
            int result = 0;


            return result;
        }

        public string addSboFields()
        {
            string result = "Ok";
            if (!sboApi.isDIConnected) sboApi.connectCompany();

            result = "Creating Configuration Table";
            
            sboApi.AddTable("NOP_CFG", "NOP Configuration Table", BoUTBTableType.bott_NoObject);
            
            sboApi.AddColumns("@NOP_CFG", "companyDb", "SBO Source Company", SAPbobsCOM.BoFieldTypes.db_Memo, 20, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "SboUID", "SBO User ID", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "SboPwd", "SBO User PWD", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "DbUserName", "DB User ID", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "DbPassword", "DB Password", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "ServerType", "DB User ID", SAPbobsCOM.BoFieldTypes.db_Memo, 5, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "SboServer", "DB Password", SAPbobsCOM.BoFieldTypes.db_Memo, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");


          

            sboApi.AddColumns("@NOP_CFG", "NopSServer", "NOP Server", SAPbobsCOM.BoFieldTypes.db_Memo, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "NopDbName", "NOP DB Name", SAPbobsCOM.BoFieldTypes.db_Memo, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "NopDbUserName", "NOP DB UID", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "NopDbPassword", "NOP Pwd", SAPbobsCOM.BoFieldTypes.db_Memo, 12, SAPbobsCOM.BoFldSubTypes.st_None, "");

            sboApi.AddColumns("@NOP_CFG", "standardPricelist", "STD Price LIst", SAPbobsCOM.BoFieldTypes.db_Memo, 5, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "whsCode", "Warehouse", SAPbobsCOM.BoFieldTypes.db_Memo, 5, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "productImageFolder", "Product Image Folder", SAPbobsCOM.BoFieldTypes.db_Memo, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "categorImageFolder", "Category Image Folder", SAPbobsCOM.BoFieldTypes.db_Memo, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");


            sboApi.AddColumns("@NOP_CFG", "SFI", "Sales Order Fetch Intervel", SAPbobsCOM.BoFieldTypes.db_Memo, 5, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "SSeries", "Sales Order Series", SAPbobsCOM.BoFieldTypes.db_Memo, 5, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "SSE", "Sales Employee", SAPbobsCOM.BoFieldTypes.db_Memo, 10, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_CFG", "SOwner", "Owner", SAPbobsCOM.BoFieldTypes.db_Memo, 10, SAPbobsCOM.BoFldSubTypes.st_None, "");


              sboApi.AddTable("NOP_LOG", "NOP Log Table", BoUTBTableType.bott_NoObject);
            sboApi.AddColumns("@NOP_LOG", "LogId", "ID", SAPbobsCOM.BoFieldTypes.db_Memo, 20, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_LOG", "DateTime", "Date Time", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_LOG", "LogBy", "Log By", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_LOG", "Action", "Action", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_LOG", "Dscription", "Description", SAPbobsCOM.BoFieldTypes.db_Memo, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_LOG", "MsgType", "Message Type", SAPbobsCOM.BoFieldTypes.db_Memo, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("@NOP_LOG", "CMNTS", "Additional Comments", SAPbobsCOM.BoFieldTypes.db_Memo, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");






            sboApi.AddColumns("OCRG", "NOPID", "NOPID", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("OCRD", "NOPID", "NOPID", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("OITB", "NOPID", "NOPID", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("OITM", "NOPID", "NOPID", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");


            sboApi.AddColumns("OITB", "Father", "Fatehr Category Code", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("OITB", "PORTAL", "Show on Web Portal", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            sboApi.AddColumns("OITB", "WebDscr", "Web Description", SAPbobsCOM.BoFieldTypes.db_Memo, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");

            return result;
        }

        public int SyncRole()
        {
            int result = 0;
            if (!sboApi.isDIConnected) sboApi.connectCompany();

            string strGetRole = "Select GroupCode, GroupName, GroupType, Locked, DataSource, UserSign, PriceList, DiscRel, U_NOPID from ocrg where grouptype='C' ";
            Hashtable sqp = new Hashtable();
               
            DataTable sboRole = dsSAP.getDataTable(strGetRole);
            foreach (DataRow dr in sboRole.Rows)
            {

                string QryalreadyExist = "Select top 1 ID from CustomerRole where IsSystemRole=0 and SystemName = '" + dr["GroupCode"].ToString() + "'";
                DataTable dtNopRole = dsNOP.getDataTable(QryalreadyExist);

                if (dtNopRole.Rows.Count > 0)
                {
                    sqp.Clear();
                    string roleId = dtNopRole.Rows[0]["ID"].ToString();

                }
                else
                {
                    sqp.Clear();
                    string strInsertRole = "INSERT CustomerRole (Name, FreeShipping, TaxExempt, Active, IsSystemRole, SystemName, PurchasedWithProductId) ";
                    strInsertRole += " VALUES (@p2,  @p3,  @p4, @p5, @p6, @p7,@p8)";
                    sqp.Add("P2", dr["GroupName"].ToString());
                    sqp.Add("P3", "0");
                    sqp.Add("P4", "0");
                    sqp.Add("P5", "1");
                    sqp.Add("P6", "0");
                    sqp.Add("P7", dr["GroupCode"].ToString());
                    sqp.Add("P8", "0");
                    dsNOP.ExecuteNonQuery(strInsertRole, sqp);
                    long RoleId = dsNOP.getMaxId("CustomerRole", "ID");

                    string CrgUpdate = " Update ocrg set U_NOPID='" + RoleId.ToString() + "' where groupcode= '" + dr["groupcode"].ToString() + "' ";
                    dsSAP.ExecuteNonQuery(CrgUpdate);

                }


               
            }


            return result;
        }

        public int SyncCustomer()
        {
            int result = 0;
            if (!sboApi.isDIConnected) sboApi.connectCompany();

            string strGetCustomer = "Select * from ocrd where cardtype='C' ";
            Hashtable sqp = new Hashtable();

            DataTable sboCustomers = dsSAP.getDataTable(strGetCustomer);
            foreach (DataRow dr in sboCustomers.Rows)
            {

                string QryalreadyExist = "Select top 1 ID from Customer where Username= '" + dr["CardCode"].ToString() + "'";
                DataTable dtNopCustomer = dsNOP.getDataTable(QryalreadyExist);

                if (dtNopCustomer.Rows.Count > 0)
                {
                    sqp.Clear();
                    string CustomerId = dtNopCustomer.Rows[0]["ID"].ToString();

                }
                else
                {
                    addSboCustomer(dr["CardCode"].ToString());

                }



            }


            return result;
        }
        public int addSboCustomer(string cardcode)
        {
            int result = 0;
            DataTable dtCustomer = dsSAP.getDataTable("Select * from ocrd where cardCode='" + cardcode + "'");
            foreach (DataRow dr in dtCustomer.Rows)
            {

                Hashtable sqp = new Hashtable();
                string strInsertCustomer = "INSERT [dbo].[Customer]([CustomerGuid], [Username], [Email], [Password], [PasswordFormatId], [PasswordSalt], [AdminComment], [IsTaxExempt], [AffiliateId], [VendorId], [HasShoppingCartItems], [Active], [Deleted], [IsSystemAccount], [SystemName], [LastIpAddress], [CreatedOnUtc],  [LastActivityDateUtc]) ";
                strInsertCustomer += " VALUES (NEWID(), @p2,  @p3,  @p4, @p5, @p6, @p7,@p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17,  @p18)";
                sqp.Add("P1", dr["CardCode"].ToString());
                sqp.Add("P2", dr["CardCode"].ToString());
                sqp.Add("P3", dr["E_Mail"].ToString());
                sqp.Add("P4", dr["Password"].ToString());
                sqp.Add("P5", "0");
                sqp.Add("P6", "");
                sqp.Add("P7", "From SAP");
                sqp.Add("P8", "0");
                sqp.Add("P9", "0");
                sqp.Add("P10", "0");
                sqp.Add("P11", "0");
                sqp.Add("P12", "1");
                sqp.Add("P13", "0");
                sqp.Add("P14", "0");
                sqp.Add("P15", "");
                sqp.Add("P16", "");
                sqp.Add("P17", DateTime.Now.ToString());
                sqp.Add("P18", DateTime.Now.ToString());
                dsNOP.ExecuteNonQuery(strInsertCustomer, sqp);

                long CustomerId = dsNOP.getMaxId("Customer", "ID");

                string CrdUpdate = " Update ocrd set U_NOPID='" + CustomerId.ToString() + "' where cardcode= '" + dr["cardcode"].ToString() + "' ";
                dsSAP.ExecuteNonQuery(CrdUpdate);


                sqp.Clear();



                string strNopRoleId = "Select top 1 ID from CustomerRole where SystemName='" + dr["GroupCode"].ToString() + "'";
                string NopRoleId = dsNOP.getScallerValue(strNopRoleId).ToString();


                string strInsertGroupMapping = "INSERT [dbo].[Customer_CustomerRole_Mapping]([Customer_Id], [CustomerRole_Id]) ";
                strInsertGroupMapping += "VALUES (@p0, @p1)";

                sqp.Add("P0",CustomerId.ToString());
                sqp.Add("P1", NopRoleId);

                dsNOP.ExecuteNonQuery(strInsertGroupMapping,sqp);
                sqp.Clear();
                strInsertGroupMapping = "INSERT [dbo].[Customer_CustomerRole_Mapping]([Customer_Id], [CustomerRole_Id]) ";
                strInsertGroupMapping += "VALUES (@p0, @p1)";

                sqp.Add("P0", CustomerId.ToString());
                sqp.Add("P1", "3");

                dsNOP.ExecuteNonQuery(strInsertGroupMapping, sqp);
                sqp.Clear();


                string strInsertFName = " INSERT [dbo].[GenericAttribute]([EntityId], [KeyGroup], [Key], [Value], [StoreId]) ";
                strInsertFName += " VALUES ('" + CustomerId.ToString() + "', 'Customer', 'FirstName', '" + dr["CardName"].ToString() + "', '0')";
                dsNOP.ExecuteNonQuery(strInsertFName);

                string strInsertLName = " INSERT [dbo].[GenericAttribute]([EntityId], [KeyGroup], [Key], [Value], [StoreId]) ";
                strInsertLName += " VALUES ('" + CustomerId.ToString() + "', 'Customer', 'LastName', '" + dr["CardCode"].ToString() + "', '0')";
                dsNOP.ExecuteNonQuery(strInsertLName);

                string billtoAddId = "";
                string shiptoAddId = "";
                string strAddressInsert = "";

                string strAddressSelect = "Select * from crd1 where cardcode='" + cardcode + "' and AdresType='B'";

                DataTable dtAddress = dsSAP.getDataTable(strAddressSelect);
                if (dtAddress.Rows.Count > 0)
                {
                    strAddressInsert = " Insert into Address(FirstName, LastName, Email, Company, CountryId, StateProvinceId, City, Address1, Address2, ZipPostalCode, PhoneNumber, CreatedOnUtc ) ";
                    strAddressInsert += " Values (@FirstName, @LastName, @Email, @Company, @CountryId, @StateProvinceId, @City, @Address1, @Address2, @ZipPostalCode, @PhoneNumber,getdate())";

                    sqp.Clear();
                    sqp.Add("FirstName", dr["CardCode"].ToString());
                    sqp.Add("LastName", dr["CntctPrsn"].ToString());
                    sqp.Add("Email", dr["E_Mail"].ToString());
                    sqp.Add("Company", dr["CardName"].ToString());
                    sqp.Add("CountryId", getAddId( dtAddress.Rows[0]["Country"].ToString(), "Country"));
                    sqp.Add("StateProvinceId", getAddId( dtAddress.Rows[0]["State"].ToString(),"State"));
                    sqp.Add("City", dtAddress.Rows[0]["City"].ToString());
                    sqp.Add("Address1", dtAddress.Rows[0]["Street"].ToString());
                    sqp.Add("Address2", dtAddress.Rows[0]["Block"].ToString());
                    sqp.Add("ZipPostalCode", dtAddress.Rows[0]["ZipCode"].ToString());
                    sqp.Add("PhoneNumber",dr["Phone1"].ToString());

                    dsNOP.ExecuteNonQuery(strAddressInsert, sqp);

                    billtoAddId = Convert.ToString( dsNOP.getMaxId("Address", "ID"));

                    dsNOP.ExecuteNonQuery("Update Customer set BillingAddress_Id = '" + billtoAddId + "' where ID = '" + CustomerId.ToString() + "'");

                    

                }
                strAddressSelect = "Select * from crd1 where cardcode='" + dr["CardCode"].ToString() + "' and AdresType='S'";
                 dtAddress = dsSAP.getDataTable(strAddressSelect);
                if (dtAddress.Rows.Count > 0)
                {
                    strAddressInsert = " Insert into Address(FirstName, LastName, Email, Company, CountryId, StateProvinceId, City, Address1, Address2, ZipPostalCode, PhoneNumber, CreatedOnUtc ) ";
                    strAddressInsert += " Values (@FirstName, @LastName, @Email, @Company, @CountryId, @StateProvinceId, @City, @Address1, @Address2, @ZipPostalCode, @PhoneNumber,getdate())";

                    sqp.Clear();
                    sqp.Add("FirstName", dr["CardCode"].ToString());
                    sqp.Add("LastName", dr["CntctPrsn"].ToString());
                    sqp.Add("Email", dr["E_Mail"].ToString());
                    sqp.Add("Company", dr["CardName"].ToString());
                    sqp.Add("CountryId", getAddId(dtAddress.Rows[0]["Country"].ToString(), "Country"));
                    sqp.Add("StateProvinceId", getAddId(dtAddress.Rows[0]["State"].ToString(), "State"));
                    sqp.Add("City", dtAddress.Rows[0]["City"].ToString());
                    sqp.Add("Address1", dtAddress.Rows[0]["Street"].ToString());
                    sqp.Add("Address2", dtAddress.Rows[0]["Block"].ToString());
                    sqp.Add("ZipPostalCode", dtAddress.Rows[0]["ZipCode"].ToString());
                    sqp.Add("PhoneNumber", dr["Phone1"].ToString());

                    dsNOP.ExecuteNonQuery(strAddressInsert, sqp);

                    shiptoAddId = Convert.ToString(dsNOP.getMaxId("Address", "ID"));

                    dsNOP.ExecuteNonQuery("Update Customer set ShippingAddress_Id = '" + shiptoAddId + "' where ID = '" + CustomerId.ToString() + "'");

                   



                }


            }

            return result;
        }

        public string getAddId(string strCode,string strType)
        {
            string outResult = "0";
            if(strType=="State")
            {
                outResult = Convert.ToString( dsNOP.getScallerValue("Select ID from StateProvince where Abbreviation='" + strCode + "'"));
            }

            if (strType == "Country")
            {
                outResult = Convert.ToString(dsNOP.getScallerValue("Select ID from Country where TwoLetterIsoCode='" + strCode + "'"));
            }


            return outResult;
        }
        public int SyncItemGroup()
        {
            int result = 0;
            if (!sboApi.isDIConnected) sboApi.connectCompany();

            string strGetItemGroup = "Select * from oitb  order by isnull(U_Father,'')  ";
            Hashtable sqp = new Hashtable();

            DataTable sboItemGroup = dsSAP.getDataTable(strGetItemGroup);
            foreach (DataRow dr in sboItemGroup.Rows)
            {

                string QryalreadyExist = "Select top 1 ID from Category where MetaTitle= '" + dr["ItmsGrpCod"].ToString() + "'";
                DataTable dtNopCategory = dsNOP.getDataTable(QryalreadyExist);

                if (dtNopCategory.Rows.Count > 0)
                {
                    sqp.Clear();
                    string CategoryId = dtNopCategory.Rows[0]["ID"].ToString();
                    UpdateNopCategory(CategoryId,"CatCode");
                }
                else
                {
                    addSboItemGroup(dr["ItmsGrpCod"].ToString());

                }



            }


            return result;
        }

        public int UpdateNopCategory(string sboCode , string updateBy)
        {
            int result = 0;
            string searchBy=" where ItmsGrpCod='" + sboCode + "'"; 
            if(updateBy=="CatName") searchBy = " where itmsgrpnam='" + sboCode + "' "; 
            DataTable dtGrp = dsSAP.getDataTable("Select * from oitb " + searchBy );

            foreach (DataRow dr in dtGrp.Rows)
            {
                string published = "0";

                string ParentCategoryId = "0";
                if (dr["U_PORTAL"].ToString() == "Y")
                {
                    published = "1";
                }
                string cagetoryNopId = dr["U_NOPID"].ToString();
                if (dr["U_Father"] != null && dr["U_Father"].ToString() != "")
                {
                    string sboParrentId = dr["U_Father"].ToString();
                    ParentCategoryId = Convert.ToString(dsNOP.getScallerValue("Select ID from Category where MetaTitle = '" + sboParrentId.ToString() + "'"));
                }

                Hashtable sqp = new Hashtable();
                string strInsertCategory = "update [dbo].[Category] set [Name] =@Name , [Description] = @Description, MetaTitle=@MetaTitle,ParentCategoryId= @ParentCategoryId , Published=@Published where id =@ID  ";
                sqp.Add("Name", dr["ItmsGrpNam"].ToString().Replace(".", " "));
                sqp.Add("Description", dr["U_WebDscr"].ToString());
                sqp.Add("MetaTitle", dr["ItmsGrpCod"].ToString());
                sqp.Add("ParentCategoryId", ParentCategoryId);
                sqp.Add("Published", published);
                sqp.Add("ID", cagetoryNopId);
                dsNOP.ExecuteNonQuery(strInsertCategory, sqp);

              



            }

            return result;
        }

      
        public int addSboItemGroup(string CategoryCode)
        {
            int result = 0;
            DataTable dtCustomer = dsSAP.getDataTable("Select * from oitb where ItmsGrpCod='" + CategoryCode + "'");
            foreach (DataRow dr in dtCustomer.Rows)
            {
                string ParentCategoryId="0";
                if(dr["U_Father"] !=null && dr["U_Father"].ToString()!="")
                {
                    string sboParrentId = dr["U_Father"].ToString();
                    ParentCategoryId = Convert.ToString( dsNOP.getScallerValue("Select ID from Category where MetaTitle = '" + sboParrentId.ToString() + "'"));
                }

                Hashtable sqp = new Hashtable();
                string strInsertCategory = "INSERT [dbo].[Category]([Name], [Description], [CategoryTemplateId], [MetaTitle], [ParentCategoryId], [PictureId], [PageSize], [AllowCustomersToSelectPageSize], [PageSizeOptions], [ShowOnHomePage], [IncludeInTopMenu], [HasDiscountsApplied], [SubjectToAcl], [LimitedToStores], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc]) ";
                strInsertCategory += " VALUES (@Name, @Description, '1', @MetaTitle, @ParentCategoryId, '0', '4', '1','8, 4, 12', '0', '1', '0', '1', '0', '1', '0','0',getdate(),getdate()) ";
                sqp.Add("Name", dr["ItmsGrpNam"].ToString().Replace(".", " "));
                sqp.Add("Description", dr["U_WebDscr"].ToString());
                sqp.Add("MetaTitle", dr["ItmsGrpCod"].ToString());
                sqp.Add("ParentCategoryId", ParentCategoryId);
                dsNOP.ExecuteNonQuery(strInsertCategory, sqp);

                long CategoryId = dsNOP.getMaxId("Category", "ID");

                string CrdUpdate = " Update oitb set U_NOPID='" + CategoryId.ToString() + "' where itmsgrpcod= '" + dr["itmsgrpcod"].ToString() + "' ";
                dsSAP.ExecuteNonQuery(CrdUpdate);


                sqp.Clear();



                sqp.Clear();
                string strInsertURLMapping = "INSERT [dbo].[UrlRecord]([EntityId], [EntityName], [Slug], [IsActive], [LanguageId]) ";
                strInsertURLMapping += " VALUES ('" + CategoryId.ToString() + "','Category', '" + dr["itmsgrpnam"].ToString() + "', '1','0')";


                dsNOP.ExecuteNonQuery(strInsertURLMapping, sqp);
                sqp.Clear();


               
            }

            return result;
        }

        public int SyncItems()
        {
            int result = 0;
            if (!sboApi.isDIConnected) sboApi.connectCompany();

            string strGetItem = "Select  oitm.* from oitm inner join oitb on oitm.itmsgrpcod=oitb.itmsgrpcod  where  oitb.U_PORTAL='Y' and  SellItem='Y'   ";
            Hashtable sqp = new Hashtable();

            DataTable sboItems = dsSAP.getDataTable(strGetItem);
            foreach (DataRow dr in sboItems.Rows)
            {

                string QryalreadyExist = "Select top 1 ID from Product where sku= '" + dr["ItemCode"].ToString() + "'";
                DataTable dtNopItem = dsNOP.getDataTable(QryalreadyExist);

                if (dtNopItem.Rows.Count > 0)
                {
                    sqp.Clear();
                    string ItemId = dtNopItem.Rows[0]["ID"].ToString();

                }
                else
                {
                    addSboItems(dr["ItemCode"].ToString());

                }



            }


            return result;
        }
        public int addSboItems(string ItemCode)
        {
            int result = 0;
            DataTable dtCustomer = dsSAP.getDataTable("Select * from oitm where ItemCode='" + ItemCode + "'");
            foreach (DataRow dr in dtCustomer.Rows)
            {
                string CategoryId = "0";
                if (dr["Itmsgrpcod"] != null && dr["Itmsgrpcod"].ToString() != "")
                {
                    string sboItemGroup = dr["Itmsgrpcod"].ToString();
                    CategoryId = Convert.ToString(dsNOP.getScallerValue("Select ID from Category where MetaTitle = '" + sboItemGroup.ToString() + "'"));
                }

                Hashtable sqp = new Hashtable();
                string strInsertItem = " INSERT [dbo].[Product]([ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [Name], [ShortDescription], [FullDescription],  [ProductTemplateId], [VendorId], [ShowOnHomePage],  [AllowCustomerReviews], [ApprovedRatingSum], [NotApprovedRatingSum], ";
                strInsertItem += "   [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores], [Sku],  [IsGiftCard], [GiftCardTypeId], [RequireOtherProducts], ";
                strInsertItem += " [AutomaticallyAddRequiredProducts], [IsDownload], [DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads],  [DownloadActivationTypeId], [HasSampleDownload], [SampleDownloadId] ";
                strInsertItem += "  , [HasUserAgreement], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles], [IsRental], [RentalPriceLength], [RentalPricePeriodId],";
                strInsertItem += "  [IsShipEnabled], [IsFreeShipping], [ShipSeparately], [AdditionalShippingCharge], [DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [IsTelecommunicationsOrBroadcastingOrElectronicServices], [ManageInventoryMethodId], ";
                strInsertItem += "    [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity], [MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], ";
                strInsertItem += "      [AllowBackInStockSubscriptions], [OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [DisableBuyButton], [DisableWishlistButton], [AvailableForPreOrder], ";
                strInsertItem += "    [CallForPrice], [Price], [OldPrice], [ProductCost],[CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], ";
                strInsertItem += "   [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc]) ";

                strInsertItem += " VALUES (5, 0, 1, @Name, @ShortDescription, @FullDescription, 1, 0, 0, 1, 0, 0, ";
                strInsertItem += " 0, 0, 0, 0, @Sku,  0, 0, 0, ";
                strInsertItem += " 0, 0, 0, 1,10,  1, 0, 0, ";
                strInsertItem += " 0,  0, 100, 0,10, 0,1,0, ";
                strInsertItem += " 1, 0, 0, 0.00, 0, 0, 0,0, 1, ";
                strInsertItem += " 0, 0, 1000, 1, 1, 0, 0, 1, 0, ";
                strInsertItem += " 0,1,10000, 0,0, 0, 0, ";
                strInsertItem += " 0, 0,0, @Cost,0, 0, 10000, 1,0,0, 0,0,0, ";
                strInsertItem += " 0, 1, 0,getdate(), getdate()) ";

                sqp.Add("Name", dr["ItemName"].ToString().Replace("." , " "));
                sqp.Add("ShortDescription", dr["ItemName"].ToString());
                sqp.Add("FullDescription", dr["UserText"].ToString());
                sqp.Add("Sku", dr["ItemCode"].ToString());
                sqp.Add("Cost", dr["AvgPrice"].ToString());
                dsNOP.ExecuteNonQuery(strInsertItem, sqp);

                long ProductId = dsNOP.getMaxId("Product", "ID");

                string CrdUpdate = " Update oitm set U_NOPID='" + CategoryId.ToString() + "' where itemcode= '" + dr["itemcode"].ToString() + "' ";
                dsSAP.ExecuteNonQuery(CrdUpdate);


                sqp.Clear();



                sqp.Clear();
                string strInsertURLMapping = "INSERT [dbo].[UrlRecord]([EntityId], [EntityName], [Slug], [IsActive], [LanguageId]) ";
                strInsertURLMapping += " VALUES ('" + ProductId.ToString() + "','Product', '" + dr["ItemName"].ToString().Replace("."," ") + "', '1','0')";


                dsNOP.ExecuteNonQuery(strInsertURLMapping, sqp);


         
                sqp.Clear();
                string strInsertCategory = "INSERT [dbo].[Product_Category_Mapping]([ProductId], [CategoryId], [IsFeaturedProduct], [DisplayOrder]) ";
                strInsertCategory += " VALUES ('" + ProductId.ToString() + "','" + CategoryId.ToString() + "', '0', '0')";


              string strResult=  dsNOP.ExecuteNonQuery(strInsertCategory, sqp);






                sqp.Clear();

                Program.objHrmsUI.oApplication.SetStatusBarMessage("Updating image for product " + ItemCode + " Using folder " + Program.productImageFolder , SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                updateProductImage(Program.productImageFolder, ItemCode, ProductId.ToString());

                Program.objHrmsUI.oApplication.SetStatusBarMessage("Updated image for product " + ItemCode, SAPbouiCOM.BoMessageTime.bmt_Medium, false);
               

            }

            return result;
        }


        public string syncCatImages(string categoryFolder)
        {
            string result = "OK";
            string strGetItemGroup = "Select * from OITB   ";
           
            DataTable sboItemGroup = dsSAP.getDataTable(strGetItemGroup);
            foreach (DataRow dr in sboItemGroup.Rows)
            {


                updateCatImage(categoryFolder, dr["ItmsGrpCod"].ToString());



            }
            return result;

        }
        public void updateCatImage(string categoryFolder, string ItemGroupCode)
        {
            string strOldCatPicture = "Select * from Picture where SeoFilename='C_" + ItemGroupCode +  "'  ";
            Hashtable sqp = new Hashtable();

            string strImageQuery = "";
            string pictureId = "0";
            DataTable dtOldPic = dsNOP.getDataTable(strOldCatPicture);
            if (dtOldPic.Rows.Count > 0)
            {
                 pictureId = dtOldPic.Rows[0]["ID"].ToString();
                strImageQuery = "Update Picture set  PictureBinary = @imgPic where ID='" + pictureId + "'";
                sqp.Add("imgPic", categoryFolder + "\\C_" + ItemGroupCode + ".jpeg");
                dsNOP.ExecuteNonQuery(strImageQuery, sqp);
           
            }
            else
            {

                strImageQuery = "insert into  Picture ( PictureBinary, MimeType, SeoFilename, IsNew) Values(@imgPic ,'image/pjpeg','C_" + ItemGroupCode + "',0 )";
                sqp.Add("imgPic", categoryFolder + "\\C_" + ItemGroupCode + ".jpeg");
                dsNOP.ExecuteNonQuery(strImageQuery, sqp);
                DataTable dtNewPic = dsNOP.getDataTable("Select * from Picture where SeoFilename='C_" + ItemGroupCode + "'");
                if (dtNewPic.Rows.Count > 0)
                {
                    pictureId = dtNewPic.Rows[0]["ID"].ToString();

                    dsNOP.ExecuteNonQuery("Update Category set PictureId='" + pictureId + "' where MetaTitle='" + ItemGroupCode + "'");

                }
            }


            

            

        }


        public string syncProductImages(string ProductFolder)
        {
            string result = "OK";
            string strGetItem = "Select * from Product   ";

            DataTable sboItemGroup = dsNOP.getDataTable(strGetItem);
            foreach (DataRow dr in sboItemGroup.Rows)
            {


                updateProductImage(ProductFolder, dr["Sku"].ToString(),dr["ID"].ToString());



            }
            return result;

        }
        public void updateProductImage(string ProductFolder, string ItemCode, string productId)
        {
            string strOldItemPicture = "Select * from Product_Picture_Mapping where ProductId='" + productId + "'  ";
            Hashtable sqp = new Hashtable();

            string strImageQuery = "";
            string pictureId = "0";
            DataTable dtOldPic = dsNOP.getDataTable(strOldItemPicture);
            if (dtOldPic.Rows.Count > 0)
            {
                pictureId = dtOldPic.Rows[0]["ID"].ToString();
                strImageQuery = "Update Picture set  PictureBinary = @imgPic where ID='" + pictureId + "'";
                sqp.Add("imgPic", ProductFolder + "\\" + ItemCode + ".jpg");
                dsNOP.ExecuteNonQuery(strImageQuery, sqp);

            }
            else
            {
                Program.objHrmsUI.oApplication.SetStatusBarMessage("Adding new picture " + ItemCode, SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                

                strImageQuery = "insert into  Picture ( PictureBinary, MimeType, SeoFilename, IsNew) Values(@imgPic ,'image/pjpeg','" + ItemCode + "',0 )";
                sqp.Add("imgPic", ProductFolder + "\\" + ItemCode + ".jpg");
             string insertResult=   dsNOP.ExecuteNonQuery(strImageQuery, sqp);

             Program.objHrmsUI.oApplication.SetStatusBarMessage("Image inserted with status " + insertResult , SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                
               
                
                DataTable dtNewPic = dsNOP.getDataTable("Select * from Picture where SeoFilename='" + ItemCode + "'");
                if (dtNewPic.Rows.Count > 0)
                {
                    pictureId = dtNewPic.Rows[0]["ID"].ToString();

                    dsNOP.ExecuteNonQuery("Insert into  Product_Picture_Mapping ( ProductId, PictureId, DisplayOrder ) Values ('" + productId + "','" + pictureId +"',0)");

                }
                Program.objHrmsUI.oApplication.SetStatusBarMessage("Image added for product " + ItemCode, SAPbouiCOM.BoMessageTime.bmt_Short,false);
            }






        }

     

        public string cleanData()
        {
            string result = "OK";
            string strCleanSql = "delete from CustomerRole where IsSystemRole=0 ";
            strCleanSql += " delete from GenericAttribute where KeyGroup='Customer' ";
            strCleanSql += " delete from Customer where id not in (1,2,3) ";
            strCleanSql += " delete from Category ";
            strCleanSql += " delete from Product ";

            strCleanSql += " delete from UrlRecord where EntityName='Product' ";
            strCleanSql += " delete from UrlRecord where EntityName='Category'";
            result = dsNOP.ExecuteNonQuery(strCleanSql);


            return result;
        }

        public string UpdateStandardPrice(string standPriceList)
        {
            string result = "OK";
            string strPriceList = "Select * from itm1 where pricelist='" + standPriceList  + "' ";
            DataTable sboPriceList = dsSAP.getDataTable(strPriceList);
            foreach (DataRow dr in sboPriceList.Rows)
            {


                string strUpdateStdPrice = " Update Product set Price='" + dr["Price"].ToString() + "' where sku='" + dr["ItemCode"].ToString() + "'";
                result = dsNOP.ExecuteNonQuery(strUpdateStdPrice);


            }
            


            return result;
        }

        public string syncPriceList()
        {
            string result = "OK";
            string strCG = "Select GroupCode, GroupName, GroupType, Locked, DataSource, UserSign, isnull(PriceList,'') as PriceList, DiscRel, U_NOPID from ocrg where isnull(u_nopId,'')<>'' ";

            DataTable sboCG = dsSAP.getDataTable(strCG);

            foreach (DataRow dr in sboCG.Rows)
            {
                string priceList = dr["PriceList"].ToString();
                if (priceList != "")
                {
                    updateGroupPrices(priceList, dr["U_NOPID"].ToString());
                }
            }

            return result;
        }
        public string updateGroupPrices(string PriceList, string RoleId)
        {
            string result = "OK";

            string strDeleteTierPrice = " Delete from TierPrice where CustomerRoleId='" + RoleId.ToString() + "'";
            dsNOP.ExecuteNonQuery(strDeleteTierPrice);

            string strPriceList = "Select * from itm1 where pricelist='" + PriceList + "' ";

            DataTable sboPriceList = dsSAP.getDataTable(strPriceList);
            foreach (DataRow dr in sboPriceList.Rows)
            {

                string strProductId = "Select ID from product where sku='" + dr["ItemCode"].ToString() + "'";
                string productId = Convert.ToString( dsNOP.getScallerValue(strProductId) );
                if (productId != null && productId != "")
                {

                    string strUpdateStdPrice = " insert into  TierPrice(ProductId, StoreId, CustomerRoleId, Quantity, Price) ";
                    strUpdateStdPrice += " Values ('" + productId.ToString() + "','0','" + RoleId + "','1','" + dr["Price"].ToString() + "' )";
                    result = dsNOP.ExecuteNonQuery(strUpdateStdPrice);

                }

            }



            return result;

        }


        public DataTable  getConfiguration()
        {
            DataTable dtOut;

            dtOut = dsSAP.getDataTable("Select * from [@NOP_CFG]");

            return dtOut;

        }
        public string updateConConfiguration(string p_companyDb, string p_SboUID, string p_SboPwd, string p_DbUserName, string p_DbPassword, string p_ServerType, string p_SboServer, string p_NopSServer, string p_NopDbName, string p_NopDbUserName, string p_NopDbPassword)
        {
            Hashtable sqp = new Hashtable();


            string strSql = "";
            string outResult = "OK";
            string strAlreadyCfg = "Select top 1 * from  [@NOP_CFG] where [Code]='001'";

            DataTable dtCfg = dsSAP.getDataTable(strAlreadyCfg);
            if (dtCfg.Rows.Count > 0)
            {
                strSql = " update    [@NOP_CFG] set  U_companyDb = '" + p_companyDb + "', U_SboUID= '" + p_SboUID + "', U_SboPwd = '" + p_SboPwd + "', U_DbUserName='" + p_DbUserName + "', U_DbPassword ='" + p_DbPassword + "' ";
                strSql += "  , U_ServerType = '" + p_ServerType + "', U_SboServer =  '" + p_SboServer + "', U_NopSServer = '" + p_NopSServer + "', U_NopDbName =  '" + p_NopDbName + "', U_NopDbUserName = '" + p_NopDbUserName + "', U_NopDbPassword ='" + p_NopDbPassword + "'";

            }
            else
            {

                strSql = " insert into   [@NOP_CFG]( [Code], [Name], U_companyDb, U_SboUID, U_SboPwd, U_DbUserName, U_DbPassword, U_ServerType, U_SboServer, U_NopSServer, U_NopDbName, U_NopDbUserName, U_NopDbPassword ) ";
                strSql += " Values ('001', '001', '" + p_companyDb + "', '" + p_SboUID + "', '" + p_SboPwd + "', '" + p_DbUserName + "', '" + p_DbPassword + "', '" + p_ServerType + "', '" + p_SboServer + "', '" + p_NopSServer + "','" + p_NopDbName + "', '" + p_NopDbUserName + "', '" + p_NopDbPassword + "') ";
            }
           
            Program.objHrmsUI.ExecQuery(strSql, "Update Connection Configuration");


            return outResult;

        }
        public string updateConfiguration(string p_companyDb, string p_SboUID, string p_SboPwd, string p_DbUserName, string p_DbPassword, string p_ServerType, string p_SboServer, string p_NopSServer, string p_NopDbName, string p_NopDbUserName, string p_NopDbPassword, string p_standardPricelist, string p_whsCode, string p_productImageFolder, string p_categorImageFolder ,     string  p_SFI, string p_SSeries, string p_SSE, string p_SOwner)
        {
            Hashtable sqp = new Hashtable();
      

            string strSql = "";
            string outResult = "OK";
            string strAlreadyCfg = "Select top 1 * from  [@NOP_CFG] where [Code]='001'";

            DataTable dtCfg = dsSAP.getDataTable(strAlreadyCfg);
            if (dtCfg.Rows.Count > 0)
            {
                strSql = " update    [@NOP_CFG] set  U_companyDb = @U_companyDb, U_SboUID= @U_SboUID, U_SboPwd = @U_SboPwd, U_DbUserName=@U_DbUserName, U_DbPassword =@U_DbPassword " ;
                strSql += "  , U_ServerType = @U_ServerType, U_SboServer =  @U_SboServer, U_NopSServer = @U_NopSServer, U_NopDbName =  @U_NopDbName, U_NopDbUserName = @U_NopDbUserName, U_NopDbPassword = @U_NopDbPassword, U_standardPricelist = @U_standardPricelist, U_whsCode = @U_whsCode, U_productImageFolder = @U_productImageFolder, U_categorImageFolder =  @U_categorImageFolder , U_SFI = @U_SFI, U_SSeries=@U_SSeries, U_SSE=@U_SSE, U_SOwner=@U_SOwner where [Code]=@Code  ";
    
            }
            else
            {

                strSql = " insert into   [@NOP_CFG]( [Code], [Name], U_companyDb, U_SboUID, U_SboPwd, U_DbUserName, U_DbPassword, U_ServerType, U_SboServer, U_NopSServer, U_NopDbName, U_NopDbUserName, U_NopDbPassword, U_standardPricelist, U_whsCode, U_productImageFolder, U_categorImageFolder ,U_SFI, U_SSeries, U_SSE, U_SOwner ) ";
                strSql += " Values (@Code, @Name, @U_companyDb, @U_SboUID, @U_SboPwd, @U_DbUserName, @U_DbPassword, @U_ServerType, @U_SboServer, @U_NopSServer, @U_NopDbName, @U_NopDbUserName, @U_NopDbPassword, @U_standardPricelist, @U_whsCode, @U_productImageFolder, @U_categorImageFolder , @U_SFI, @U_SSeries, @U_SSE, @U_SOwner) ";
            }
            sqp.Add("Code", "001");
            sqp.Add("Name", "001");
            sqp.Add("U_companyDb", p_companyDb);
            sqp.Add("U_SboUID", p_SboUID);
            sqp.Add("U_SboPwd", p_SboPwd);
            sqp.Add("U_DbUserName", p_DbUserName);
            sqp.Add("U_DbPassword", p_DbPassword);
            sqp.Add("U_ServerType", p_ServerType);
            sqp.Add("U_SboServer", p_SboServer);
            sqp.Add("U_NopSServer", p_NopSServer);
            sqp.Add("U_NopDbName", p_NopDbName);
            sqp.Add("U_NopDbUserName", p_NopDbUserName);
            sqp.Add("U_NopDbPassword", p_NopDbPassword);

            sqp.Add("U_standardPricelist", p_standardPricelist);
            sqp.Add("U_whsCode", p_whsCode);
            sqp.Add("U_productImageFolder", p_productImageFolder);
            sqp.Add("U_categorImageFolder", p_categorImageFolder);

           

            sqp.Add("U_SFI", p_SFI);
            sqp.Add("U_SSeries", p_SSeries);
            sqp.Add("U_SSE", p_SSE);
            sqp.Add("U_SOwner", p_SOwner);

            dsSAP.ExecuteNonQuery(strSql, sqp);


            return outResult;
        }

  }
}

