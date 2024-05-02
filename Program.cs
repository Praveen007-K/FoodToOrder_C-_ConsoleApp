using FoodToOrderLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;

// Import Restaurant Data from Json
List<Restaurant> restaurants;
using (StreamReader r = new StreamReader("Restaurant.json"))
{
    string json = r.ReadToEnd();
    restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(json);
}


//Display Board
var DisplayBoard = () => {

    Console.WriteLine("\t\t\t\t\t|------------------------------|");
    Console.WriteLine("\t\t\t\t\t|  WELCOME TO FOODTOORDER APP  |");
    Console.WriteLine("\t\t\t\t\t|______________________________|");
};

//Inputing Details
DisplayBoard();
Console.Write("For Logging in\n\n");
Console.Write("Enter your email : ");
string email = Console.ReadLine();
Console.Write("Enter the password : ");
string password = Console.ReadLine();
Console.WriteLine();

//Event Handler
void Alert()
{
    Console.WriteLine("Price Change!!!");
}

//Main Function
Login();
Console.WriteLine("\nEnter the restaurant id for the menu!");
int restaurant_id = Convert.ToInt32(Console.ReadLine());
Console.Clear();
DisplayDishes(restaurant_id);
string ans = "n";
List<CartDetail> cartDetails = new List<CartDetail>();
do
{
    Console.WriteLine("\nEnter the dish id for the menu!");
    int dish_id = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("\nEnter the quantity of required dish!");
    int dish_qty = Convert.ToInt32(Console.ReadLine());

    cartDetails.Add(new CartDetail(1, dish_id, dish_qty));

    Console.WriteLine("\nDo you wish to buy more? (y/n)");
    ans = Console.ReadLine();
}
while (ans == "y");

Console.WriteLine("\nEnter y to buy & n to cancel.");
string buy = Console.ReadLine();
Console.Clear();
if (buy == "y")
{
    GenerateBill(cartDetails, restaurant_id);
}




//Login Option
void Login()
{
    int fail = 0;
    Data d = new Data();
    foreach (var item in Data.users)
    {
        if (fail >= Data.users.Count - 1)
        {
            Console.WriteLine("\t\t\t\t\t\tLogin Failed!!!");
            Environment.Exit(1);
        }
        if (email == item.Email && password == item.Password)
        {
            item.Greetings();
            Thread.Sleep(1000);
            if (item.Role == "User")
            {
                Console.WriteLine("\t\t\t\t\t\t|||Login Success|||");
                Console.Clear();
                DisplayRestaurant();
                break;
            }
            else
            {
                AdminPanelOptions();
                Environment.Exit(1);
            }
        }
        else
        {
            fail += 1;
        }
    }
}

//Display Available Restaurant
void DisplayRestaurant()
{
    DisplayBoard();
    Console.WriteLine("\tRESTAURANTS\n");
    foreach (var item in restaurants)
    {
        if (item.Open)
        {
            Console.WriteLine(item.Id + " :  " + item.RName);

        }
    }
}

//Display Dishes from selected restaurant
void DisplayDishes(int id)
{
    DisplayBoard();
    Console.WriteLine("\n\tMENU\n");
    int index = 1;
    var query = from restaurant in restaurants where restaurant.Id == id select restaurant;
    var selected_restaurant = query.ToList()[0];
    foreach (var dish in selected_restaurant.dishes)
    {
        Console.WriteLine(index + " :  " + dish.DishName);
        index++;
    }
}

//Generate Bill & Display it to User
void GenerateBill(List<CartDetail> carts, int rest_id)
{

    DisplayBoard();
    var query1 = from restaurant in restaurants where restaurant.Id == rest_id select restaurant;
    var selected_restaurant = query1.ToList()[0];
    Console.WriteLine("\n------------------------------- BILL -------------------------------");
    Console.WriteLine("|   Dish Name    |   Unit Price   |    Quantity    |     Amount     |");
    int space = 16;
    double totalAmount = 0;
    foreach (var cart in carts)
    {
        var dishname = selected_restaurant.dishes[cart.DishId - 1].DishName;
        var price = selected_restaurant.dishes[cart.DishId - 1].Price;
        var qty = cart.Quantity;
        var amount = price * qty;

        int spaceDishName = space - dishname.Length;
        int spacePrice = space - price.ToString().Length;
        int spaceQty = space - qty.ToString().Length;
        int spaceAmount = space - amount.ToString().Length;

        Console.Write("|" + dishname + new string(' ', spaceDishName) + "|");
        Console.Write(price + new string(' ', spacePrice) + "|");
        Console.Write(qty + new string(' ', spaceQty) + "|");
        Console.Write(amount + new string(' ', spaceAmount) + "|");
        Console.WriteLine();
        totalAmount += amount;
    }
    Console.WriteLine("|-------------------------------------------------------------------|");
    Console.WriteLine("\tTotal Amount : " + totalAmount);
    Console.WriteLine("|-------------------------------------------------------------------|");
}

//Admin Options
void AdminPanelOptions()
{
    Console.Clear();
    DisplayBoard();
    Console.WriteLine("Enter the required option.");
    Console.WriteLine("1 Add Restaurant\n2 Edit Restaurant\n3 Delete Restaurant\n4 Exit\n");
    int option = Convert.ToInt32(Console.ReadLine());
    if (option == 1)
    {
        AddRestaurant();
    }
    else if (option == 2)
    {
        EditRestaurant();
    }
    else if (option==3) 
    {
        DeleteRestaurant();
    }
    else
    {
        Environment.Exit(0);
    }
}

void AddRestaurant()
{

    Console.Clear();
    DisplayBoard();
    int did;
    string dname;
    string avail;
    bool y;
    double price;
    Dish dish;
    string md;
    Console.WriteLine("Restaurant Details\n");
    Console.Write("Restaurant Id : ");
    int rid = Convert.ToInt32(Console.ReadLine());
    Console.Write("Restaurant Name : ");
    string rname = Console.ReadLine();
    Console.Write("Is it open : ");
    string open = Console.ReadLine();
    bool x = false;
    if (open == "y")
    {
        x = true;
    }

    List<Dish> dishes = new List<Dish>();
    Console.WriteLine("Dishes Details\n");
    do
    {
        Console.Clear();
        DisplayBoard();
        Console.Write("Dish Id : ");
        did = Convert.ToInt32(Console.ReadLine());
        Console.Write("Dish Name : ");
        dname = Console.ReadLine();
        Console.Write("Available : ");
        avail = Console.ReadLine();
        y = false;
        if (avail == "y")
        {
            y = true;
        }
        Console.Write("Unit Price : ");
        price = Convert.ToInt32(Console.ReadLine());
        dish = new Dish(did, dname, price, y, rid);
        dishes.Add(dish);

        Console.WriteLine("\nAdd more dishes?");
        md = Console.ReadLine();
    }
    while (md == "y");
    Restaurant r = new Restaurant(rid, rname, x, dishes);
    string jsonString = File.ReadAllText("Restaurant.json");
    var list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    list.Add(r);
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant details added successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    AdminPanelOptions();
}
void EditRestaurant()
{
    Console.Clear();
    DisplayBoard();
    Console.WriteLine("Enter the required option.");
    Console.WriteLine("1 Change Restaurant Name\n2 Change Open Status\n" +
        "3 Add Dish\n4 Edit Dish\n5 Delete Dish\n6 Exit\n");
    int option = Convert.ToInt32(Console.ReadLine());
    if (option == 1)
    {
        ChangeRestName();
    }
    else if (option == 2)
    {
        ChangeOpenStatus();
    }
    else if (option == 3)
    {
        AddDish();
    }
    else if (option == 4)
    {
        EditDish();
    }
    else if (option == 5)
    {
        DeleteDish();
    }
    else
    {
        Environment.Exit(0);
    }
}
void DeleteRestaurant()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Restaurant Id to Delete : ");
    int dltId = Convert.ToInt32(Console.ReadLine());
    string jsonString = File.ReadAllText("Restaurant.json");
    List<Restaurant> list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    Restaurant r = list.Where(x=>x.Id == dltId).ToArray()[0];
    list.Remove(r);
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant Deleted successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void ChangeRestName()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Restaurant Id to Change Name : ");
    int oldId = Convert.ToInt32(Console.ReadLine());
    Console.Write("\nEnter the New Name : ");
    string newName = Console.ReadLine();
    string jsonString = File.ReadAllText("Restaurant.json");
    var list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach(var item in list)
    {
        if (item.Id == oldId)
        {
            item.RName = newName;
        }
    }
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant name changed successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void ChangeOpenStatus()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Restaurant Id to Change Open Status : ");
    int oldId = Convert.ToInt32(Console.ReadLine());
    string jsonString = File.ReadAllText("Restaurant.json");
    var list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach (var item in list)
    {
        if (item.Id == oldId)
        {
            if (item.Open == true)
            {
                item.Open = false;
            }
            else
            {
                item.Open = true;
            }
        }
    }
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant name changed successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void AddDish()
{
    Console.Clear();
    DisplayBoard();
    Console.WriteLine("Dishes Details\n");
    Console.Write("Restaurant Id : ");
    int rId = Convert.ToInt32(Console.ReadLine());
    Console.Write("Dish Id : ");
    int dId = Convert.ToInt32(Console.ReadLine());
    Console.Write("Dish Name : ");
    string dName = Console.ReadLine();
    Console.Write("Unit Price : ");
    int price = Convert.ToInt32(Console.ReadLine());
    Console.Write("Is it available : ");
    string avail = Console.ReadLine();
    bool x = false;
    if (avail == "y")
    {
        x = true;
    }
    Dish dish = new Dish(dId, dName, price, x, rId);

    string jsonString = File.ReadAllText("Restaurant.json");
    List<Restaurant> list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach(var item in list)
    {
        if(item.Id == rId)
        {
            item.dishes.Add(dish);
        }
    }
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant details added successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void EditDish()
{
    Console.Clear();
    DisplayBoard();
    Console.WriteLine("1 Change Dish Name\n2 Change Available Status\n3 Update Price\n");
    int option = Convert.ToInt32(Console.ReadLine());
    if (option == 1)
    {
        ChangeDishName();
    }
    else if (option == 2)
    {
        ChangeAvailableStatus();
    }
    else
    {
        UpdatePrice();
    }
}

void DeleteDish()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Restaurant Id to Delete : ");
    int dltRId = Convert.ToInt32(Console.ReadLine());
    Console.Write("\nEnter the Dish Id to Delete : ");
    int dltdId = Convert.ToInt32(Console.ReadLine());
    string jsonString = File.ReadAllText("Restaurant.json");
    List<Restaurant> list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach (var item in list)
    {
        if (item.Id == dltRId)
        {
            List<Dish> dish = item.dishes;
            Dish d = dish.Where(x => x.Id == dltdId).ToArray()[0];
            dish.Remove(d);
        }
    }
    
    
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant name changed successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void ChangeDishName()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Dish Id to Change Name : ");
    int oldId = Convert.ToInt32(Console.ReadLine());
    Console.Write("\nEnter the New Name : ");
    string newName = Console.ReadLine();
    string jsonString = File.ReadAllText("Restaurant.json");
    var list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach (var item in list)
    {
        foreach (var dish in item.dishes)
        {
            if (dish.Id == oldId) {
                dish.DishName = newName; break;
            }
        }
    }
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant name changed successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void ChangeAvailableStatus()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Dish Id to Update Available Status : ");
    int oldId = Convert.ToInt32(Console.ReadLine());
    string jsonString = File.ReadAllText("Restaurant.json");
    var list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach (var item in list)
    {
        foreach (var dish in item.dishes)
        {
            if (dish.Id == oldId)
            {
                if (dish.Available)
                {
                    dish.Available = false;
                }
                else
                {
                    dish.Available = true;
                }
            }
        }
    }
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant name changed successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}

void UpdatePrice()
{
    Console.Clear();
    DisplayBoard();
    Console.Write("\nEnter the Dish Id to Update Price : ");
    int oldId = Convert.ToInt32(Console.ReadLine());
    double oldPrice;
    Console.Write("\nEnter the Updated Price : ");
    int price = Convert.ToInt32(Console.ReadLine());
    string jsonString = File.ReadAllText("Restaurant.json");
    var list = JsonConvert.DeserializeObject<List<Restaurant>>(jsonString);
    foreach (var item in list)
    {
        foreach (var dish in item.dishes)
        {
            if (dish.Id == oldId)
            {
                oldPrice = dish.Price;
                dish.Price = price;
                if (dish.Price != oldPrice)
                {
                    dish.PriceDrop += Alert;
                    dish.Alerter();
                    Thread.Sleep(1000);
                }
                    
                break;
            }
        }
    }
    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
    File.WriteAllText("C:\\Users\\Praveen K Nair\\source\\repos\\FoodToOrder\\Restaurant.json", convertedJson);

    Console.WriteLine("\nRestaurant name changed successfully...");
    Thread.Sleep(1000);
    Console.Clear();
    EditRestaurant();
}