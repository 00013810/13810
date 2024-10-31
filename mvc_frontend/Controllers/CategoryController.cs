using mvc_frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace mvc_frontend.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category +
        public async Task<ActionResult> Index()
        {
            //Hosted web API REST Service base url 
            string Baseurl = "https://localhost:44335/";
            List<Category> ProdInfo = new List<Category>();
            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync("api/Category");

                //Checking the response is successful or not which is sent HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var PrResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing the Product list
                    ProdInfo = JsonConvert.DeserializeObject<List<Category>>(PrResponse);
                }
                //returning the Product list to view
                return View(ProdInfo);
            }
        }

        // GET: Category/Details/5 +
        public async Task<ActionResult> Details(int id)
        {
            // Hosted web API REST Service base url
            string Baseurl = "https://localhost:44335/";
            Category category = null;

            using (var client = new HttpClient())
            {
                // Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();

                // Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync($"api/Category/{id}");

                // Checking the response is successful or not which is sent HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    // Storing the response details received from web api
                    var catResponse = await Res.Content.ReadAsStringAsync();

                    // Deserializing the response received from web api and storing the Category object
                    category = JsonConvert.DeserializeObject<Category>(catResponse);
                }
            }

            // Returning the Category object to the view
            return View(category);
        }


        // GET: Category/Create +
        public ActionResult Create()
        {
            return View();
        }
        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44335/"); // Your API base URL
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Serialize the category object to JSON
                    var json = JsonConvert.SerializeObject(category);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    // Send a POST request to the API to create a new category
                    var response = await client.PostAsync("api/Category", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully created, redirect to the Index action
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error creating category. Please try again.");
                    }
                }
            }

            // Return the view with the model if there are validation errors
            return View(category);
        }



        // GET: Category/Edit/5 +
        public async Task<ActionResult> Edit(int id)
        {
            Category category = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/"); // Your API base URL
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Sending a GET request to retrieve the category details
                var response = await client.GetAsync($"api/Category/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var catResponse = await response.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<Category>(catResponse);
                }
                else
                {
                    return HttpNotFound();
                }
            }

            // Return the view with the retrieved category
            return View(category);
        }

        // POST: Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44335/"); // Your API base URL
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Serialize the category object to JSON
                    var json = JsonConvert.SerializeObject(category);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    // Send a PUT request to the API to update the category
                    var response = await client.PutAsync($"api/Category/{category.ID}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successfully updated, redirect to the Index action
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error updating category. Please try again.");
                    }
                }
            }

            // Return the view with the model if there are validation errors
            return View(category);
        }
    
    // GET: Category/Delete/5 +
    private async Task<Category> GetCategoryById(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/"); // Your API base URL
                var response = await client.GetAsync($"api/Category/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<Category>(content);
                    return category; // Return the deserialized category object
                }
                return null; // Return null if the category was not found
            }
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Assuming you have a method to get the category by ID
            var category = await GetCategoryById(id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/");

                // Send a DELETE request to the API
                var response = await client.DeleteAsync($"api/Category/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Redirect to the Index action if successful
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the error if deletion fails
                    ModelState.AddModelError(string.Empty, "Error deleting category.");
                    return View(); // Return the view with the error message
                }
            }
        }

    }
}
