﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Aspect.Domain;
using Aspect.Model.ProductDomain;
using Aspect.Model.ConfigurationDomain;

namespace Aspect.UI.Web.Json
{
    public partial class ExchangeConfiguration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // выбираем значения из post запроса
            Guid[] prods = Request["products"].Split(',').Select(p => new Guid(p)).ToArray(); //120420
           // List<Guid> prods = Request["products"].Split(',').Select(p => new Guid(p)).ToList();
            Guid srcProductID = new Guid(Request["left_uid"]);
            Guid dstProductID = new Guid(Request["right_uid"]);
           //List<string> qdus_new = Request["qdus_new"].Split(',').ToList<string>(); //120420
             string[] qdus_new = Request["qdus_new"].Split(',').ToArray(); //120420

            // для каждого продукта в списке [products] выполняем 
            // замену исходного продукта на целевой
           // foreach (Guid prod in prods) //120420
             for (int i = 0; i < prods.Length; i++) //120420

            {
                 using (ProductProvider provider = new ProductProvider())
                {
                    
                      // создаём новую версию продукта
                    Guid newprod = provider.CreateNewVersionOfProduct(prods[i], Guid.Empty);

                    // берём все исходные детали для замены
                    // их может быть несколько
                    List<Aspect.Domain.Configuration> srcConfs, dstConfs;                     
                    srcConfs = provider.Configurations.Where(p => p.ProductOwnerID == newprod && p.ProductID == srcProductID).ToList();
                    dstConfs = new List<Aspect.Domain.Configuration>();

                    // создаём на базе исходных деталей - целевые с такими же параметрами
                    using (ConfigurationProvider confProvider = new ConfigurationProvider())
                    {
                        foreach (Aspect.Domain.Configuration conf in srcConfs)
                        {
                            Aspect.Domain.Configuration newconf = confProvider.CopyConfiguration(conf);
                            newconf.ID = Guid.NewGuid();
                            newconf.ProductID = dstProductID;
                            newconf.Quantity = Convert.ToDecimal(qdus_new[i]);
                            dstConfs.Add(newconf);
                        }
                    }

                    // удаляем из спецификации исходные продукты
                    // вставляем в спецификацию новые продукты
                    provider.Configurations.DeleteAllOnSubmit(srcConfs);
                    provider.Configurations.InsertAllOnSubmit(dstConfs);
                    provider.SubmitChanges();
                    
                    // устанавливаем признак основной версии
                    ProductProperty prop = provider.ProductProperties.Where(
                        p => p.ProductID == newprod &&
                        p.PropertyID == new Guid("BBE170B0-28E4-4738-B365-1038B03F4552")).Single();
                    prop.Value = "1";
                    provider.SubmitChanges();
                }
            }
        }
    }
}
