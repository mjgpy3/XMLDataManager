#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 21:21:59 EDT 2012
# 
# 

import GBaseExceptions

class XMLReader:
	def GetEntireFile(self, fileName):
		try:
			return open(fileName, 'r').read()
		except IOError:
			raise GBaseExceptions.GBaseGeneralException('Could not open \"' + fileName + '\"')			
