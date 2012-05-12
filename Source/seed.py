#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Sat May 12 09:22:18 EDT 2012
# 
# 
from GBaseHandler import GBaseHandler as g

a = g()

a.GenGBase("TestGBase")
a.GenTable("Drinks")
a.GenTable("Foods")

a.GenColIn("DrinkName", "string", "Drinks")
a.GenColIn("DrinkType", "string", "Drinks")
a.GenColIn("Flavors", "int", "Drinks")
a.GenColIn("FoodName", "string", "Foods")
a.GenColIn("CanBeBoughtAt", "string", "Foods")
a.GenColIn("TastesLike", "string", "Foods")

a.GenRowIn(["DrinkName", "DrinkType", "Flavors" ], ["Monster Coffee", "Coffee", "3"], "Drinks")
a.GenRowIn(["DrinkName", "DrinkType", "Flavors" ], ["Monster", "Energy Drink", "5" ], "Drinks")
a.GenRowIn(["DrinkName", "DrinkType", "Flavors" ], ["Mountain Dew", "Soda", "6" ], "Drinks")
a.GenRowIn(["DrinkName", "DrinkType", "Flavors" ], [ "Milk", "Natural", "2" ], "Drinks")
a.GenRowIn(["FoodName", "CanBeBoughtAt", "TastesLike" ], ["Pizza", "Papa Johns", "Pepers" ], "Foods")
a.GenRowIn(["FoodName", "CanBeBoughtAt", "TastesLike" ], ["Taco", "Taco Bell", "Chicken" ], "Foods")
a.GenRowIn(["FoodName", "CanBeBoughtAt", "TastesLike" ], ["Ice Cream", "DQ", "Sugar" ], "Foods")
a.GenRowIn(["FoodName", "CanBeBoughtAt", "TastesLike" ], ["Steak", "Ponderosa", "Chicken" ], "Foods")
a.GenRowIn(["FoodName", "CanBeBoughtAt", "TastesLike" ], ["Cookies", "Subway", "Chicken" ], "Foods")
a.GenRowIn(["CanBeBoughtAt", "FoodName", "TastesLike" ], ["WalMart", "Sausage", "Chicken" ], "Foods")

