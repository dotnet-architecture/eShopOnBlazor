﻿using eShopLegacyWebForms.Models;
using eShopLegacyWebForms.Services;
using log4net;
using System;
using System.Collections.Generic;

namespace eShopLegacyWebForms.Catalog
{
    public partial class Create : System.Web.UI.Page
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ICatalogService CatalogService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _log.Info($"Now loading... /Catalog/Create.aspx");
        }

        public IEnumerable<CatalogBrand> GetBrands()
        {
            return CatalogService.GetCatalogBrands();
        }

        public IEnumerable<CatalogType> GetTypes()
        {
            return CatalogService.GetCatalogTypes();
        }

        protected void Create_Click(object sender, EventArgs e)
        {
            if (this.ModelState.IsValid)
            {
                //get the file name of the posted image  
                string imgName = PictureUpload.FileName;
                //sets the image path  
                string imgPath = "~/Pics/" + imgName;
                PictureUpload.SaveAs(Server.MapPath(imgPath));
                var catalogItem = new CatalogItem
                {
                    Name = Name.Text,
                    Description = Description.Text,
                    CatalogBrandId = int.Parse(Brand.SelectedValue),
                    CatalogTypeId = int.Parse(Type.SelectedValue),
                    Price = decimal.Parse(Price.Text),
                    PictureFileName = imgName,
                    PictureUri = imgPath,
                    AvailableStock = int.Parse(Stock.Text),
                    RestockThreshold = int.Parse(Restock.Text),
                    MaxStockThreshold = int.Parse(Maxstock.Text)
                };

                CatalogService.CreateCatalogItem(catalogItem);

                Response.Redirect("~");
            }
        }
    }
}