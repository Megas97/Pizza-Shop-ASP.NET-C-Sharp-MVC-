using PizzaShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PizzaShop.Controllers
{
    public class HomeController : Controller
    {
        #region // Index Action
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region // About Action
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
        #endregion

        #region // Contacts Action
        [HttpGet]
        public ActionResult Contacts()
        {
            ViewBag.Message = "Address";

            return View();
        }
        #endregion

        #region // Show Pizzas Action
        [HttpGet]
        public ActionResult ShowPizzas()
        {
            string message = "";
            string ingredients = "";
            int availablePizzas = 0;
            List<Pizza> pizzasList = new List<Pizza>();
            using (DBEntities de = new DBEntities())
            {
                if (de.Pizzas.Count() > 0)
                {
                    foreach (var pizza in de.Pizzas)
                    {
                        if (pizza != null)
                        {
                            pizzasList.Add(pizza);
                            ingredients = "";
                            var recipesIngredientsForRecipe = pizza.Recipe.RecipesIngredients.Where(a => a.RecipeID == pizza.RecipeID);
                            int i = 0;
                            foreach (var item in recipesIngredientsForRecipe)
                            {
                                string separator = "";
                                if (i < recipesIngredientsForRecipe.Count() - 1)
                                {
                                    separator = ", ";
                                }
                                else
                                {
                                    separator = "";
                                }
                                ingredients += item.Ingredient.IngredientName + separator;
                                i++;
                            }
                            ViewData.Add(pizza.PizzaName + " " + pizza.PizzaSize, ingredients);
                            availablePizzas++;
                        }
                    }
                }
            }

            if (availablePizzas == 1)
            {
                message = "There is " + availablePizzas + " pizza available!";
            }
            else if (availablePizzas > 1)
            {
                message = "There are " + availablePizzas + " pizzas available!";
            }
            else
            {
                message = "There are no pizzas available!";
            }

            if (TempData["showPizzasOrderInfo"] != null)
            {
                ViewBag.Message2 = TempData["showPizzasOrderInfo"].ToString();
            }

            ViewBag.Message = message;
            return View(pizzasList);
        }
        #endregion

        #region // Create Order With Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult CreateOrderWithPizza(int id)
        {
            string message = "";
            bool canUploadNew = false;
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order anything until you verify your account!";
                }
                else
                {
                    var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                    if (pizza != null)
                    {
                        if (de.Orders.Count() > 0)
                        {
                            foreach (var ordr in de.Orders)
                            {
                                if (ordr != null)
                                {
                                    if ((ordr.Pizza.PizzaName.Equals(pizza.PizzaName)) && (ordr.Pizza.PizzaSize.Equals(pizza.PizzaSize)))
                                    {
                                        ordr.PizzaAmount += 1;
                                        message = "Pizza order successfully updated!";
                                        canUploadNew = false;
                                        break;
                                    }
                                    else
                                    {
                                        canUploadNew = true;
                                    }
                                }
                            }
                            de.SaveChanges();
                            if (canUploadNew == true)
                            {
                                Order order = new Order();
                                order.Pizza = pizza;
                                if (currentUser != null)
                                {
                                    order.User = currentUser;
                                }
                                order.PizzaAmount = 1;
                                de.Orders.Add(order);
                                message = "Pizza successfully ordered!";
                                de.SaveChanges();
                            }
                        }
                        else
                        {
                            Order order = new Order();
                            order.Pizza = pizza;
                            if (currentUser != null)
                            {
                                order.User = currentUser;
                            }
                            order.PizzaAmount = 1;
                            de.Orders.Add(order);
                            message = "Pizza successfully ordered!";
                            de.SaveChanges();
                        }
                    }
                }
            }

            TempData["showPizzasOrderInfo"] = message;
            return RedirectToAction("ShowPizzas", "Home");
        }
        #endregion

        #region // Checkout Action
        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                List<Order> ordersList = new List<Order>();
                double totalOrderPrice = 0.0;
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                var ordersByCurrentUser = de.Orders.Where(a => a.UserID == currentUser.UserID);
                foreach (var order in ordersByCurrentUser)
                {
                    ordersList.Add(order);

                    if (!ViewData.ContainsKey("pizzaPicturePath" + order.OrderID))
                    {
                        ViewData.Add("pizzaPicturePath" + order.OrderID, order.Pizza.PizzaPicturePath);
                    }

                    if (!ViewData.ContainsKey("pizzaName" + order.OrderID))
                    {
                        ViewData.Add("pizzaName" + order.OrderID, order.Pizza.PizzaName);
                    }

                    if (!ViewData.ContainsKey("pizzaSize" + order.OrderID))
                    {
                        ViewData.Add("pizzaSize" + order.OrderID, order.Pizza.PizzaSize);
                    }

                    if (!ViewData.ContainsKey("orderPrice" + order.OrderID))
                    {
                        ViewData.Add("orderPrice" + order.OrderID, order.Pizza.PizzaPrice * order.PizzaAmount); // Order price = pizza price multiplied by pizza amount.
                    }
                    totalOrderPrice += order.Pizza.PizzaPrice * order.PizzaAmount;
                }

                if (!ViewData.ContainsKey("totalOrderPrice"))
                {
                    ViewData.Add("totalOrderPrice", totalOrderPrice);
                }

                if (ordersByCurrentUser.Count() == 1)
                {
                    message = "There is " + ordersByCurrentUser.Count().ToString() + " pizza in your cart!";
                }else if (ordersByCurrentUser.Count() > 1)
                {
                    message = "There are " + ordersByCurrentUser.Count().ToString() + " pizzas in your cart!";
                }
                else
                {
                    message = "Your cart is empty!";
                }

                ViewBag.Message = message;
                ViewBag.Message2 = TempData["orderInfo"];
                return View(ordersList);
            }
        }
        #endregion

        #region // Add Order Action
        [Authorize]
        [HttpGet]
        public ActionResult AddOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot update your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        message = "Order #" + order.OrderID + " was successfully updated!";
                        order.PizzaAmount += 1;
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // View Order Action
        [Authorize]
        [HttpGet]
        public ActionResult ViewOrder(int id)
        {
            string message = "";
            Pizza pizza = null;
            using (DBEntities de = new DBEntities())
            {
                var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                if (order != null)
                {
                    message = "Information about order #" + order.OrderID;
                    pizza = order.Pizza;
                    ViewBag.Quantity = order.PizzaAmount;
                    ViewBag.TotalPrice = order.Pizza.PizzaPrice * order.PizzaAmount;
                    ViewBag.Message = message;
                    return View(pizza);
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // Remove Order Action
        [Authorize]
        [HttpGet]
        public ActionResult RemoveOrder(int id)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot remove your orders until you verify your account!";
                }
                else
                {
                    var order = de.Orders.Where(a => a.OrderID == id).FirstOrDefault();
                    if (order != null)
                    {
                        if (order.PizzaAmount > 1)
                        {
                            message = "Order #" + order.OrderID + " was successfully updated!";
                            order.PizzaAmount -= 1;
                        }
                        else
                        {
                            // Remove any custom recipe and pizza the user may have created.
                            var pizza = order.Pizza;
                            var recipe = pizza.Recipe;
                            if (pizza != null)
                            {
                                if (pizza.PizzaName.Contains("Custom Pizza #"))
                                {
                                    de.Pizzas.Remove(pizza);
                                    if (recipe != null)
                                    {
                                        var recipeIngredients = recipe.RecipesIngredients;
                                        de.RecipesIngredients.RemoveRange(recipeIngredients);
                                        de.Recipes.Remove(recipe);
                                    }
                                }
                            }
                            message = "Order #" + order.OrderID + " was successfully removed!";
                            de.Orders.Remove(order);
                        }
                        de.SaveChanges();
                    }
                }
            }

            TempData["orderInfo"] = message;
            return RedirectToAction("Checkout", "Home");
        }
        #endregion

        #region // Checkout Completed Action
        [Authorize]
        [HttpGet]
        public ActionResult CheckoutCompleted()
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                var currentUserEmailIdentity = HttpContext.User.Identity.Name;
                var currentUser = de.Users.Where(a => a.EmailID == currentUserEmailIdentity).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot checkout until you verify your account!";
                }
                else
                {
                    var ordersByCurrentUser = de.Orders.Where(a => a.UserID == currentUser.UserID);
                    if (ordersByCurrentUser.Count() > 0)
                    {
                        foreach (var order in ordersByCurrentUser)
                        {
                            if (order != null)
                            {
                                if (order.Pizza.PizzaName.Contains("Custom Pizza #"))
                                {
                                    // Remove any custom recipe and pizza the user may have created.
                                    var pizza = order.Pizza;
                                    var recipe = pizza.Recipe;
                                    if (pizza != null)
                                    {
                                        if (pizza.PizzaName.Contains("Custom Pizza #"))
                                        {
                                            de.Pizzas.Remove(pizza);
                                        }
                                    }
                                    if (recipe != null)
                                    {
                                        var recipeIngredients = recipe.RecipesIngredients;
                                        de.RecipesIngredients.RemoveRange(recipeIngredients);
                                        de.Recipes.Remove(recipe);
                                    }
                                }
                                de.Orders.Remove(order);
                            }
                        }
                        message = "Your orders are on their way to your address at " + currentUser.Address + "!";
                        de.SaveChanges();
                    }
                    else
                    {
                        message = "You have not placed any orders yet!";
                    }
                }
            }

            ViewBag.Message = message;
            return View();
        }
        #endregion

        #region // Create Custom Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult CreateCustomPizza(int id)
        {
            using (DBEntities de = new DBEntities())
            {
                var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                if (pizza != null)
                {
                    ViewBag.Message = TempData["customOrderInfo"];
                    return View(pizza);
                }
            }

            ViewBag.Message = TempData["customOrderInfo"];
            return View();
        }
        #endregion

        #region // Create Order With Custom Pizza POST Action
        [Authorize]
        [HttpPost]
        public ActionResult CreateOrderWithCustomPizza(int id, FormCollection formCollection)
        {
            string message = "";
            int savedID = id;
            bool ingredientsSelected = false;
            using (DBEntities de = new DBEntities())
            {
                User currentUser = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (currentUser.IsEmailVerified != true)
                {
                    message = "You cannot order a custom pizza until you verify your account!";
                }
                else
                {
                    if (formCollection["CustomPizzaSizesList"] == "")
                    {
                        TempData["customOrderInfo"] = "Pizza size is required!";
                        return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID });
                    }

                    var pizza = de.Pizzas.Where(a => a.PizzaID == id).FirstOrDefault();
                    Recipe recipe = new Recipe();
                    Pizza customPizza = new Pizza();
                    de.SaveChanges();
                    string pizzaName = "Custom Pizza #" + pizza.PizzaID + " | User #" + currentUser.UserID + " | Date: " + DateTime.Now.ToString() + " | " + pizza.PizzaName;
                    string recipeName = "Custom Recipe #" + recipe.RecipeID + " | User #" + currentUser.UserID + " | Date: " + DateTime.Now.ToString() + " | " + pizza.Recipe.RecipeName;
                    double ingredientsPrice = 0.0;
                    double pizzaPrice = 0.0;
                    string pizzaSize = "";
                    string imagePath = pizza.PizzaPicturePath;
                    recipe.RecipeName = recipeName;
                    de.Recipes.Add(recipe);
                    de.SaveChanges();

                    if (formCollection != null)
                    {
                        pizzaSize = formCollection["CustomPizzaSizesList"];
                    }

                    foreach (var checkbox in formCollection)
                    {
                        foreach (var ingredient in de.Ingredients)
                        {
                            if (checkbox != null)
                            {
                                string isChecked = formCollection[checkbox.ToString()];
                                if (isChecked == "true,false") // This means the checkbox is selected, "false" means it's not selected.
                                {
                                    Ingredient ingr = de.Ingredients.Where(a => a.IngredientName.Equals(checkbox.ToString())).FirstOrDefault();
                                    if (ingr != null)
                                    {
                                        RecipesIngredient recpIngr = new RecipesIngredient();
                                        recpIngr.Recipe = recipe;
                                        recpIngr.Ingredient = ingr;
                                        recipe.RecipesIngredients.Add(recpIngr);
                                        ingredientsSelected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (ingredientsSelected == false)
                    {
                        TempData["customOrderInfo"] = "Pizza ingredients are required!";
                        return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID });
                    }

                    foreach (var ingr in recipe.RecipesIngredients)
                    {
                        if (ingr != null)
                        {
                            ingredientsPrice += ingr.Ingredient.IngredientPrice;
                        }
                    }

                    // Pizza price = Ingredients Price + 50% of ingredients price (overcharge, salaries) + size increase.
                    if (pizzaSize.Equals("Small"))
                    {
                        pizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice);
                    }
                    else if (pizzaSize.Equals("Medium"))
                    {
                        pizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.2;
                    }
                    else if (pizzaSize.Equals("Large"))
                    {
                        pizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.4;
                    }

                    customPizza.PizzaName = pizzaName;
                    customPizza.Recipe = recipe;
                    customPizza.PizzaPrice = pizzaPrice;
                    customPizza.PizzaSize = pizzaSize;
                    customPizza.PizzaPicturePath = imagePath;
                    de.Pizzas.Add(customPizza);
                    de.SaveChanges();
                    Order order = new Order();
                    order.Pizza = customPizza;
                    order.PizzaAmount = 1;
                    order.User = currentUser;
                    de.Orders.Add(order);
                    de.SaveChanges();
                    message = "Your custom pizza has been added to cart!";
                }
            }

            TempData["customOrderInfo"] = message;
            return RedirectToAction("CreateCustomPizza", "Home", new { id = savedID });
        }
        #endregion
    }
}