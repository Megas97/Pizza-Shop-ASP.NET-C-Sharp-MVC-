using PizzaShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzaShop.Controllers
{
    public class AdminController : Controller
    {
        #region // Access Denied Action
        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region // Admin Panel Action
        [Authorize]
        [HttpGet]
        public ActionResult AdminPanel()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Ingredient Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadIngredient()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Ingredient POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadIngredient(Ingredient ingredient)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload ingredients until you verify your account!"; ;
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;
                                    if (ModelState.IsValid)
                                    {
                                        if (ingredient != null)
                                        {
                                            if (de.Ingredients.Count() < 1)
                                            {
                                                canUpload = true;
                                            }
                                            else
                                            {
                                                foreach (var ingr in de.Ingredients)
                                                {
                                                    if (ingr.IngredientName.Equals(ingredient.IngredientName))
                                                    {
                                                        canUpload = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        canUpload = true;
                                                    }
                                                }
                                            }

                                            if (canUpload == true)
                                            {
                                                de.Ingredients.Add(ingredient);
                                                de.SaveChanges();
                                                message = "Ingredient uploaded successfully!";
                                            }
                                            else
                                            {
                                                message = "This ingredient is already present in the database!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Invalid Request!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View(ingredient);
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Recipe Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadRecipe()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Recipe POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadRecipe(Recipe recipe, FormCollection formCollection)
        {
            string message = "";
            bool ingredientsAdded = false;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload recipes until you verify your account!";
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;
                                    if (ModelState.IsValid)
                                    {
                                        if (recipe != null)
                                        {
                                            if (de.Recipes.Count() < 1)
                                            {
                                                canUpload = true;
                                            }
                                            else
                                            {
                                                foreach (var recp in de.Recipes)
                                                {
                                                    if (recp.RecipeName.Equals(recipe.RecipeName))
                                                    {
                                                        canUpload = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        canUpload = true;
                                                    }
                                                }
                                            }

                                            if (canUpload == true)
                                            {
                                                de.Recipes.Add(recipe);
                                                de.SaveChanges();
                                                var recp = de.Recipes.Where(a => a.RecipeName.Equals(recipe.RecipeName)).FirstOrDefault();
                                                if (recp != null)
                                                {
                                                    if (formCollection != null)
                                                    {
                                                        foreach (var checkbox in formCollection)
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
                                                                        recpIngr.Recipe = recp;
                                                                        recpIngr.Ingredient = ingr;
                                                                        recp.RecipesIngredients.Add(recpIngr);
                                                                        de.SaveChanges();
                                                                        message = "Recipe uploaded successfully!";
                                                                        ingredientsAdded = true;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (ingredientsAdded == false)
                                                        {
                                                            message = "You must select at least one ingredient!";
                                                            de.Recipes.Remove(recipe);
                                                            de.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                message = "This recipe is already present in the database!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Invalid Request!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View(recipe);
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Pizza Action
        [Authorize]
        [HttpGet]
        public ActionResult UploadPizza()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Upload Pizza POST Action
        [Authorize]
        [HttpPost]
        public ActionResult UploadPizza(Pizza pizza, FormCollection formCollection, HttpPostedFileBase Image)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        ViewBag.Message = "You cannot upload pizzas until you verify your account!";
                        return View();
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    bool canUpload = false;

                                    string selectedRecipeString = formCollection["RecipesList"];
                                    Recipe selectedRecipe = de.Recipes.Where(a => a.RecipeName.Equals(selectedRecipeString)).FirstOrDefault();
                                    double ingredientsPrice = 0.0;

                                    if ((selectedRecipe == null) && (formCollection["SizesList"] == "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza recipe, pizza size and pizza image are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe == null) && (formCollection["SizesList"] == "") && (Image != null))
                                    {
                                        ViewBag.Message = "Pizza recipe and pizza size are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe == null) && (formCollection["SizesList"] != "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza recipe and pizza image are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe != null) && (formCollection["SizesList"] == "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza size and pizza image are required";
                                        return View();
                                    }
                                    else if ((selectedRecipe == null) && (formCollection["SizesList"] != "") && (Image != null))
                                    {
                                        ViewBag.Message = "Pizza recipe is required";
                                        return View();
                                    }
                                    else if ((selectedRecipe != null) && (formCollection["SizesList"] == "") && (Image != null))
                                    {
                                        ViewBag.Message = "Pizza size is required";
                                        return View();
                                    }
                                    else if ((selectedRecipe != null) && (formCollection["SizesList"] != "") && (Image == null))
                                    {
                                        ViewBag.Message = "Pizza image is required";
                                        return View();
                                    }

                                    // Check if the image file is of a supported extension.
                                    string ext = Path.GetExtension(Image.FileName);
                                    if ((ext.Equals(".xbm")) || (ext.Equals(".bmp")) || (ext.Equals(".jpeg")) || (ext.Equals(".webp")) || (ext.Equals(".svgz")) || (ext.Equals(".gif")) || (ext.Equals(".jfif")) || (ext.Equals(".png")) || (ext.Equals(".svg")) || (ext.Equals(".jpg")) || (ext.Equals(".ico")) || (ext.Equals(".tiff")) || (ext.Equals(".pjpeg")) || (ext.Equals(".pjp")) || (ext.Equals(".tif")))
                                    {
                                        if (selectedRecipe != null)
                                        {
                                            int selectedRecipeID = selectedRecipe.RecipeID;
                                            pizza.Recipe = selectedRecipe;
                                            var recipesIngredientsForRecipe = pizza.Recipe.RecipesIngredients.Where(a => a.RecipeID == selectedRecipeID);
                                            foreach (var item in recipesIngredientsForRecipe)
                                            {
                                                ingredientsPrice += item.Ingredient.IngredientPrice;
                                            }
                                        }

                                        if (formCollection["SizesList"] != "")
                                        {
                                            pizza.PizzaSize = formCollection["SizesList"];
                                        }

                                        // Pizza price = Ingredients Price + 50% of ingredients price (overcharge, salaries) + size increase.
                                        if (pizza.PizzaSize.Equals("Small"))
                                        {
                                            pizza.PizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice);
                                        }
                                        else if (pizza.PizzaSize.Equals("Medium"))
                                        {
                                            pizza.PizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.2;
                                        }
                                        else if (pizza.PizzaSize.Equals("Large"))
                                        {
                                            pizza.PizzaPrice = (ingredientsPrice + 0.5 * ingredientsPrice) + (ingredientsPrice + 0.5 * ingredientsPrice) * 0.4;
                                        }

                                        pizza.PizzaPicturePath = ""; // We make the path be empty because we just need it to be initialized and later we'll set the real value.

                                        if (de.Pizzas.Count() < 1)
                                        {
                                            canUpload = true;
                                        }
                                        else
                                        {
                                            foreach (var item in de.Pizzas)
                                            {
                                                if (item.PizzaName.Equals(pizza.PizzaName) && (item.PizzaSize.Equals(pizza.PizzaSize)))
                                                {
                                                    canUpload = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    canUpload = true;
                                                }
                                            }
                                        }

                                        if (canUpload == true)
                                        {
                                            if (ModelState.IsValid)
                                            {
                                                de.Pizzas.Add(pizza);
                                                de.SaveChanges();
                                                string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                                                string extension = Path.GetExtension(Image.FileName);
                                                // We get the actual uploaded pizza's id and set it as value for the path attribute
                                                int pizzaIndex = pizza.PizzaID;
                                                pizza.PizzaPicturePath = "/Uploads/" + pizzaIndex + extension; // Video File Name == ID (Unique!)
                                                Image.SaveAs(Server.MapPath(pizza.PizzaPicturePath));
                                                de.SaveChanges();
                                                message = "Pizza uploaded successfully!";
                                            }
                                            else
                                            {
                                                message = "Invalid Request!";
                                            }
                                        }
                                        else
                                        {
                                            message = "This pizza is already present in the database!";
                                        }
                                    }
                                    else
                                    {
                                        message = "The selected image is not of a supported file format! " + ext;
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Manage Users Action
        [Authorize]
        [HttpGet]
        public ActionResult ManageUsers()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                var allUsers = de.Users.ToList();
                                ViewBag.Message = TempData["giveAdminText"];
                                ViewBag.Message2 = TempData["revokeAdminText"];
                                ViewBag.Message3 = TempData["deleteUserText"];
                                return View(allUsers);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Give Admin POST Action
        [Authorize]
        [HttpPost]
        public ActionResult GiveAdmin(FormCollection formCollection)
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["giveAdminText"] = "You cannot give admin access until you verify your account!";
                        return RedirectToAction("ManageUsers", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["UsersList"] != "")
                                    {
                                        string userEmail = formCollection["UsersList"].ToString();
                                        User selectedUser = de.Users.Where(a => a.EmailID == userEmail).FirstOrDefault();
                                        if (selectedUser != null)
                                        {
                                            var selectedUserRole = selectedUser.UsersRoles.FirstOrDefault();
                                            Role role = de.Roles.Where(a => a.RoleName.Equals("Admin")).FirstOrDefault();
                                            selectedUserRole.User = selectedUser;
                                            selectedUserRole.Role = role;
                                            selectedUser.UsersRoles.Add(selectedUserRole);
                                            de.SaveChanges();
                                            TempData["giveAdminText"] = "User with email '" + userEmail + "' was given admin access!";
                                        }
                                    }
                                    else
                                    {
                                        TempData["giveAdminText"] = "Please select a user to which to give admin access to first!";
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Revoke Admin POST Action
        [Authorize]
        [HttpPost]
        public ActionResult RevokeAdmin(FormCollection formCollection)
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["revokeAdminText"] = "You cannot revoke admin access until you verify your account!";
                        return RedirectToAction("ManageUsers", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["UsersList"] != "")
                                    {
                                        string userEmail = formCollection["UsersList"].ToString();
                                        User selectedUser = de.Users.Where(a => a.EmailID == userEmail).FirstOrDefault();
                                        if (selectedUser != null)
                                        {
                                            var selectedUserRole = selectedUser.UsersRoles.FirstOrDefault();
                                            Role role = de.Roles.Where(a => a.RoleName.Equals("User")).FirstOrDefault();
                                            selectedUserRole.User = selectedUser;
                                            selectedUserRole.Role = role;
                                            selectedUser.UsersRoles.Add(selectedUserRole);
                                            de.SaveChanges();
                                            TempData["revokeAdminText"] = "User with email '" + userEmail + "' was revoked admin access!";
                                        }
                                    }
                                    else
                                    {
                                        TempData["revokeAdminText"] = "Please select a user from which to revoke admin access first!";
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete User POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteUser(FormCollection formCollection)
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        TempData["deleteUserText"] = "You cannot delete users until you verify your account!";
                        return RedirectToAction("ManageUsers", "Admin");
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["UsersList"] != "")
                                    {
                                        string userEmail = formCollection["UsersList"].ToString();
                                        User selectedUser = de.Users.Where(a => a.EmailID == userEmail).FirstOrDefault();
                                        if (selectedUser != null)
                                        {
                                            var userOrders = de.Orders.Where(a => a.UserID == selectedUser.UserID);
                                            foreach (var order in userOrders)
                                            {
                                                if (order != null)
                                                {
                                                    de.Orders.Remove(order);
                                                }
                                            }
                                            var userRoleToDelete = de.UsersRoles.Where(a => a.UserID == selectedUser.UserID).FirstOrDefault();
                                            de.UsersRoles.Remove(userRoleToDelete);
                                            de.Users.Remove(selectedUser);
                                            de.SaveChanges();
                                            TempData["deleteUserText"] = "User with email '" + userEmail + "' was deleted!";
                                        }
                                    }
                                    else
                                    {
                                        TempData["deleteUserText"] = "Please select a user which to delete first!";
                                    }
                                }
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Ingredient Action
        [Authorize]
        [HttpGet]
        public ActionResult DeleteIngredient()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Ingredient POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteIngredient(FormCollection formCollection)
        {
            string message = "";
            bool ingredientUsed = false;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete ingredients until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["IngredientsList"] != "")
                                    {
                                        string ingredientName = formCollection["IngredientsList"].ToString();
                                        Ingredient ingredient = de.Ingredients.Where(a => a.IngredientName == ingredientName).FirstOrDefault();
                                        if (ingredient != null)
                                        {
                                            int ingredientID = ingredient.IngredientID;
                                            foreach (var recpIngr in de.RecipesIngredients)
                                            {
                                                if (recpIngr.IngredientID == ingredientID)
                                                {
                                                    ingredientUsed = true;
                                                    break;
                                                }
                                            }

                                            if (ingredientUsed == false)
                                            {
                                                message = "Ingredient '" + ingredient.IngredientName + "' successfully deleted!";
                                                de.Ingredients.Remove(ingredient);
                                                de.SaveChanges();
                                            }
                                            else
                                            {
                                                message = "This ingredient is being used at the moment so it cannot be deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select an ingredient from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Recipe Action
        [Authorize]
        [HttpGet]
        public ActionResult DeleteRecipe()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Recipe POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeleteRecipe(FormCollection formCollection)
        {
            string message = "";
            bool recipeUsed = false;
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete recipes until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["RecipesList"] != "")
                                    {
                                        string recipeName = formCollection["RecipesList"].ToString();
                                        Recipe recipe = de.Recipes.Where(a => a.RecipeName == recipeName).FirstOrDefault();
                                        if (recipe != null)
                                        {
                                            foreach (Pizza pizza in de.Pizzas)
                                            {
                                                if (pizza.Recipe == recipe)
                                                {
                                                    recipeUsed = true;
                                                    break;
                                                }
                                            }

                                            if (recipeUsed == false)
                                            {
                                                message = "Recipe '" + recipe.RecipeName + "' successfully deleted!";
                                                var recipeIngredients = recipe.RecipesIngredients;
                                                de.RecipesIngredients.RemoveRange(recipeIngredients);
                                                de.Recipes.Remove(recipe);
                                                de.SaveChanges();
                                            }
                                            else
                                            {
                                                message = "This recipe is being used at the moment so it cannot be deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select a recipe from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Pizza
        [Authorize]
        [HttpGet]
        public ActionResult DeletePizza()
        {
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    var userRoles = user.UsersRoles;
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role.RoleName.Equals("Admin"))
                        {
                            if (userRole.UserID == user.UserID)
                            {
                                return View();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion

        #region // Delete Pizza POST Action
        [Authorize]
        [HttpPost]
        public ActionResult DeletePizza(FormCollection formCollection)
        {
            string message = "";
            using (DBEntities de = new DBEntities())
            {
                User user = de.Users.Where(a => a.EmailID == HttpContext.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsEmailVerified != true)
                    {
                        message = "You cannot delete pizzas until you verify your account!";
                    }
                    else
                    {
                        var userRoles = user.UsersRoles;
                        foreach (var userRole in userRoles)
                        {
                            if (userRole.Role.RoleName.Equals("Admin"))
                            {
                                if (userRole.UserID == user.UserID)
                                {
                                    if (formCollection["PizzasList"] != "")
                                    {
                                        string[] pizzaNameAndSize = formCollection["PizzasList"].ToString().Split(new string[] { " - " }, StringSplitOptions.None);
                                        string pizzaName = pizzaNameAndSize[0];
                                        string pizzaSize = pizzaNameAndSize[1];
                                        var pizza = de.Pizzas.Where(a => (a.PizzaName == pizzaName) && (a.PizzaSize == pizzaSize)).FirstOrDefault();
                                        if (pizza != null)
                                        {
                                            int pizzaID = pizza.PizzaID;
                                            var ordersWithPizza = de.Orders.Where(a => a.PizzaID == pizzaID);
                                            string imagePath = pizza.PizzaPicturePath;
                                            if (System.IO.File.Exists(Request.MapPath(imagePath)))
                                            {
                                                foreach (var order in ordersWithPizza)
                                                {
                                                    if (order.PizzaID == pizza.PizzaID)
                                                    {
                                                        de.Orders.Remove(order); // Delete the order which contains the selected pizza as well, otherwise there'll be an error.
                                                    }
                                                }
                                                de.Pizzas.Remove(pizza);
                                                de.SaveChanges();
                                                System.IO.File.Delete(Request.MapPath(imagePath));
                                                message = "Pizza '" + pizza.PizzaName + " - " + pizza.PizzaSize + "' successfully deleted!";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        message = "Please select a pizza from the list first!";
                                    }
                                }
                            }
                        }
                    }

                    ViewBag.Message = message;
                    return View();
                }
            }

            return RedirectToAction("AccessDenied", "Admin");
        }
        #endregion
    }
}