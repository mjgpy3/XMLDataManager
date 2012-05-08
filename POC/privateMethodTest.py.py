#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 22:21:31 EDT 2012
# 
# 

class Foo:
	def __init__(self, value):
		self.side = value

	def CalculateArea(self):
		self.Area = self.__multiply(self.side, self.side)

	def __multiply(self, num1, num2):
		return num1 * num2


a = Foo(4)

a.CalculateArea()

print a.Area
