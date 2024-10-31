using mvc_frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace mvc_frontend.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product Infromation -- work +
        public async Task<ActionResult> Index()
        {
            //Hosted web API REST Service base url 
            string Baseurl = "https://localhost:44335/";
            List<Product> ProdInfo = new List<Product>();
            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync("api/Product");

                //Checking the response is successful or not which is sent HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var PrResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing the Product list
                    ProdInfo = JsonConvert.DeserializeObject<List<Product>>(PrResponse);
                }
                //returning the Product list to view
                return View(ProdInfo);
            }
        }

        // GET: Product/Details -- work +
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            string Baseurl = "https://localhost:44335/";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/Product/{id}");

                if (response.IsSuccessStatusCode)
                {
                    Product product = await response.Content.ReadAsAsync<Product>();
                    return View(product);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        // GET: Product/Create +
        public ActionResult Create()
        {
            return View();
        }
        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44335/"); // Your API base URL
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Serialize the product object to JSON
                    var json = JsonConvert.SerializeObject(product);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    // Send a POST request to the API to create a new product
                    var response = await client.PostAsync("api/Product", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully created, redirect to the Index action
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error creating product. Please try again.");
                    }
                }
            }

            // Return the view with the model if there are validation errors
            return View(product);
        }

        // Edit Product/Edit
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve the original product information
            Product originalProduct = await GetProductById(id.Value);

            if (originalProduct == null)
            {
                return HttpNotFound();
            }

            return View(originalProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product updatedProduct)
        {
            if (ModelState.IsValid)
            {
                string Baseurl = "https://localhost:44335/";

                // Retrieve the original product information to compare category
                Product originalProduct = await GetProductById(updatedProduct.ID);

                if (originalProduct == null)
                {
                    return HttpNotFound();
                }

                // Check if the category has changed
                bool isCategoryChanged = originalProduct.ProductCategory.ID != updatedProduct.ProductCategory.ID;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Create a new Product object with only the necessary updates
                    var productToUpdate = new Product
                    {
                        ID = updatedProduct.ID,
                        Name = updatedProduct.Name,
                        Price = updatedProduct.Price,
                        ProductCategory = isCategoryChanged ? updatedProduct.ProductCategory : originalProduct.ProductCategory // Only update CategoryId if changed
                    };

                    // Serialize the updated product object to JSON
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(productToUpdate), Encoding.UTF8, "application/json");

                    // Send PUT request to update the product
                    HttpResponseMessage response = await client.PutAsync($"api/Product/{productToUpdate.ID}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error updating product.");
                    }
                }
            }

            return View(updatedProduct);
        }



        // GET: Product/Delete -- work +
        private async Task<Product> GetProductById(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/");
                var response = await client.GetAsync($"api/Product/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Product>();
                }
                else
                {
                    return null; // Or handle the error appropriately, e.g., log the error
                }
            }
        }
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await GetProductById(id.Value);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/");
                var response = await client.DeleteAsync($"api/Product/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error deleting product.");
                    return View();
                }
            }
        }
    }
}

