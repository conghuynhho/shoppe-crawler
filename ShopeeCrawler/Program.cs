using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FileHelpers;

namespace ShopeeCrawler
{
    class Program
    {
        public WoocommerceProducts mapDataToWooCommerce(ShoppeProducts inputProduct)
        {
            WoocommerceProducts outputProduct = new WoocommerceProducts();
            outputProduct.ID = "";
            outputProduct.Type = "simple";
            outputProduct.SKU = "";
            outputProduct.Name = inputProduct.Title;
            outputProduct.Published = "1";
            outputProduct.Isfeatured = "0";
            outputProduct.VisibilityInCatalog = "visible";
            outputProduct.ShortDescription = "";
            outputProduct.Description = inputProduct.Description;
            outputProduct.DateSalePriceStarts = "";
            outputProduct.DateSalePriceEnds = "";
            outputProduct.TaxStatus = "taxable";
            outputProduct.TaxClass = "";
            outputProduct.InStock = "10";
            outputProduct.Stock = "";
            outputProduct.LowStockAmount = "";
            outputProduct.BackordersAllowed = "0";
            outputProduct.SoldIndividually = "0";
            outputProduct.Weight = "";
            outputProduct.Height = "";
            outputProduct.Length = "";
            outputProduct.Width = "";
            outputProduct.AllowCustomerReviews = "1";
            outputProduct.PurchaseNote = "";
            outputProduct.SalePrice = "";
            outputProduct.RegularPrice = inputProduct.RegularPrice;
            outputProduct.Categories = inputProduct.Categories;
            outputProduct.Tags = "";
            outputProduct.Shipping = "";
            outputProduct.Images = inputProduct.Images;
            outputProduct.DownloadLimit = "";
            outputProduct.DownloadExpiryDays = "";
            outputProduct.Parent = "";
            outputProduct.GroupedProducts = "";
            outputProduct.Upsells = "";
            outputProduct.CrossSells = "";
            outputProduct.ExternalURL = "";
            outputProduct.ButtonText = "";
            outputProduct.Position = "0";

            return outputProduct;
        }


        static void Main(string[] args)
        {
            IWebDriver browser = new ChromeDriver();

            browser.Navigate().GoToUrl("https://shopee.vn");

            

            System.Threading.Thread.Sleep(2000);
            var closePopUpBtn = browser.FindElement(By.CssSelector(".shopee-popup__close-btn"));

            closePopUpBtn.Click();

            var searchBox = browser.FindElement(By.CssSelector(".shopee-searchbar-input__input"));

            searchBox.SendKeys("ốp lưng iphone");

            searchBox.SendKeys(Keys.Enter);

            List<string> listProductLink = new List<string>();

            System.Threading.Thread.Sleep(2000);

            // move to bottom element to load full page
            Actions action = new Actions(browser);
            var bottomElement = browser.FindElement(By.CssSelector(".Pca2IN"));
            action.MoveToElement(bottomElement);
            action.Perform();

            System.Threading.Thread.Sleep(4000);


            var productsElements = browser.FindElements(By.CssSelector(".shopee-search-item-result__item"));

            foreach(var element in productsElements)
            {
                var aTag = element.GetAttribute("innerHTML");

                string link = Regex.Match(aTag, "href=\"(.*?)\"").Groups[1].Value;

                listProductLink.Add("https://shopee.vn" + link);

                
            }

            List<WoocommerceProducts> listProduct = new List<WoocommerceProducts>();


            foreach (var link in listProductLink)
            {
                if(link != "https://shopee.vn")
                {
                    browser.Navigate().GoToUrl(link);

                    // Wait for the internet
                    System.Threading.Thread.Sleep(2000);

                    // move to bottom element to load full page
                    Actions action2 = new Actions(browser);
                    var bottomElement2 = browser.FindElement(By.CssSelector(".Pca2IN"));
                    action2.MoveToElement(bottomElement2);
                    action2.Perform();


                    ShoppeProducts result = new ShoppeProducts();

                    // Product title
                    string html = browser.PageSource;

                    string productTitle = Regex.Match(html, "@type\":\"Product\",\"name\":\"(.*?)\"").Groups[1].Value;

                    // Size

                    // Color
                    //Giá sản phẩm

                    string productPrice = browser.FindElement(By.CssSelector("._3e_UQT")).Text.Replace("₫", "");

                    //string productPrice = Regex.Match(html, "current-price item-card-special__current-price--special\">₫(.*?)</div></").Groups[1].Value;

                    if (productPrice.Contains("-"))
                    {
                        productPrice = Regex.Match(productPrice, "(.*?) -").Groups[1].Value;
                    }

                    //Ảnh sản phẩm 
                    List<string> productImageList = new List<string>();
                    var listImageElement = browser.FindElements(By.CssSelector("._12uy03"));
                    string productImage = "";
                    if (listImageElement.Count > 0)
                    {
                        foreach (var imageElement in listImageElement)
                        {
                            string style = imageElement.GetAttribute("style");
                            string bg = Regex.Match(style, "background-image: url\\(\"(.*?)_tn\"\\); background-size: contain; background-repeat: no-repeat;").Groups[1].Value;
                            productImageList.Add(bg);

                        }
                        productImage = String.Join(", ", productImageList.ToArray());

                    }

                    //Danh mục
                    List<string> listCategory = new List<string>();
                    var categories = browser.FindElements(By.CssSelector("._3YDLCj._3LWINq"));
                    string productCategory = "";
                    for (int j = 1; j < categories.Count; j++)
                    {
                        string categoryItem;
                        if (j != categories.Count - 1)
                        {
                            categoryItem = categories[j].Text + ">";
                        }
                        else
                        {
                            categoryItem = categories[j].Text;
                        }
                        productCategory += categoryItem;
                    }

                    //Thương hiệu
                    string productBrand = browser.FindElements(By.XPath("//label[text()='Thương hiệu']/following-sibling::div"))[0].Text;

                    //Gửi từ
                    /*var productSendFromElement = browser.FindElements(By.XPath("//label[text()='Gửi từ']/following-sibling::div"));
                    string productSendFrom = "";
                    if(productSendFromElement.Count > 0)
                    {
                        productSendFrom = productSendFromElement[0].Text;
                    }*/

                    //Mô tả sản phẩm
                    string productDescription = browser.FindElements(By.XPath("//div[@class='_3yZnxJ']/child::span"))[0].Text;
                    productDescription = productDescription.Replace("\n", " ").Replace("\r", " ").Replace("\r\n", " ");


                    //Đánh giá sản phẩm
                    //string productRating = browser.FindElement(By.CssSelector(".product-rating-overview__rating-score")).Text;
                    //string rating = Regex.Match(html, "\"ratingValue\":\"(\\d.*?)\"}}</sc").Groups[1].Value;

                    //Link sản phẩm

                    // Add all information to Shopee Object
                    result.Title = productTitle;
                    result.RegularPrice = productPrice;
                    result.ProductBrand = productBrand;
                    result.Categories = productCategory;
                    result.Images = productImage;

                    Program program = new Program();

                    listProduct.Add(program.mapDataToWooCommerce(result));
                    System.Threading.Thread.Sleep(1000);
                }
                
            }
            var engine = new FileHelperEngine<WoocommerceProducts>();
            engine.WriteFile("F:/output.txt", listProduct);











            /**
            //Create an instance of Chrome driver
            IWebDriver browser = new ChromeDriver();
            //Navigate to website Tiki.vn > Laptop category
            browser.Navigate().GoToUrl("https://tiki.vn/laptop/c8095");
            //Select all product items by CSS Selector
            List<string> listProductLink = new List<string>();
            var products = browser.FindElements(By.CssSelector(".product-item"));
            foreach (var product in products)
            {
                string outerHtml = product.GetAttribute("outerHTML");
                string productLink = Regex.Match(outerHtml, "href=\"(.*?)\"").Groups[1].Value;
                if (productLink.Contains("tka.tiki.vn"))
                {
                    productLink = "https:" + productLink;
                    listProductLink.Add(productLink);
                }
                else
                {
                    productLink = "https://tiki.vn" + productLink;
                    listProductLink.Add(productLink);
                }
            }

            List<ProductWooCommerce> listProduct = new List<ProductWooCommerce>();
            //Go to each product link
            for (int i = 0; i <= listProductLink.Count - 1; i++)
            {
                ProductTiki result = new ProductTiki();
                //Go to product link
                browser.Navigate().GoToUrl(listProductLink[i]);
                System.Threading.Thread.Sleep(1000);
                //Extract product information by CSS Selector

                string productTitle = browser.FindElements(By.CssSelector(".title"))[0].Text;

                //Extract product brand by CSS Selector then remove redundant data by Regular Expression
                string productBrand = browser.FindElements(By.CssSelector(".brand-and-author"))[0].GetAttribute("outerHTML");
                productBrand = Regex.Match(productBrand, "brand\">(.*?)</a>").Groups[1].Value;
                //Extract product price
                System.Threading.Thread.Sleep(1800);
                var priceMode = browser.FindElements(By.CssSelector(".flash-sale-price"));
                if (priceMode.Count > 0)
                {
                    string currentPrice = browser.FindElements(By.XPath("//div[@class='flash-sale-price']/child::span"))[0].Text;
                    string listPrice = browser.FindElements(By.CssSelector(".list-price"))[0].Text;
                    result.SalePrice = currentPrice;
                    result.RegularPrice = listPrice;
                }
                else
                {
                    //var x = browser.FindElements(By.CssSelector(".product-price__current-price"));
                    string currentPrice = browser.FindElements(By.CssSelector(".product-price__current-price"))[0].Text;
                    var listPriceElement = browser.FindElements(By.CssSelector(".product-price__list-price"));
                    string listPrice = listPriceElement.Count > 0 ? listPriceElement[0].Text : currentPrice;
                    result.SalePrice = currentPrice;
                    result.RegularPrice = listPrice;
                }
                //Extract product sku
                string productSKU = browser.FindElements(By.XPath("//td[text()='SKU']/parent::tr"))[0].GetAttribute("outerHTML");
                productSKU = Regex.Matches(productSKU, "<td>(.*?)</td>")[1].Groups[1].Value;
                //Extract product images
                string productImage = browser.FindElements(By.CssSelector(".PictureV2__StyledImage-tfuu67-1"))[0].GetAttribute("src");
                //Extract product category
                List<string> listCategory = new List<string>();
                var categories = browser.FindElements(By.CssSelector(".breadcrumb-item"));
                string productCategory = "";
                for (int j = 1; j < categories.Count - 1; j++)
                {
                    string categoryItem;
                    if (j != categories.Count - 2)
                    {
                        categoryItem = categories[j].Text + ">";
                    }
                    else
                    {
                        categoryItem = categories[j].Text;
                    }
                    productCategory += categoryItem;
                }
                //Extract product description
                var viewMoreElement = browser.FindElements(By.CssSelector(".btn-more"));
                if (viewMoreElement.Count > 0)
                {
                    viewMoreElement[0].Click();
                    System.Threading.Thread.Sleep(800);
                }
                string productDescription = browser.FindElements(By.CssSelector(".ToggleContent__View-sc-1hm81e2-0"))[0].Text;
                productDescription = productDescription.Replace("\n", " ").Replace("\r", " ").Replace("\r\n", " ");
                result.SKU = productSKU;
                result.Title = productTitle;
                result.Description = productDescription;
                result.Categories = productCategory;
                result.Images = productImage;
                Program program = new Program();

                listProduct.Add(program.mapDataToWooCommerce(result));
                System.Threading.Thread.Sleep(1000);
            }
            var engine = new FileHelperEngine<ProductWooCommerce>();
            engine.WriteFile("F:/output.txt", listProduct);
            **/


            //Console.WriteLine(products.Count);
            //System.IO.StreamWriter writer = new System.IO.StreamWriter("D:\\tiki.csv", false, System.Text.Encoding.UTF8);
            //writer.WriteLine("ProductName\tImageLink");
            ////System.Threading.Thread.Sleep(10000);
            ////string productLink = product.GetAttribute("href");
            ////string productName = product.FindElement(By.CssSelector(".product-item .name")).Text;
            ////string innerHtml = product.GetAttribute("innerHTML");
            //string productName = Regex.Match(outerHtml, "alt=\"(.*?)\"").Groups[1].Value;
            //string productThumbnail = Regex.Match(outerHtml, "<img src=\"(.*?)\"").Groups[1].Value;
            //writer.WriteLine(productName + "\t" + productThumbnail);
            //writer.Close();

            //browser.FindElements(By.CssSelector(".title"))[0].Text;
            //browser.FindElements(By.CssSelector(".title"))[0].GetAttribute("");
        }
    }
}
