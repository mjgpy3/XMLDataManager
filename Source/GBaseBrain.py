#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Tue May  8 14:46:12 EDT 2012
# 
# 

import GBaseExceptions as ex

class GBaseBrain:
	def FileIsTable(self, fileName, valueToLookFor = "table"):
		readData = ""
		try:
			with open(fileName, 'r') as f:
				readData = f.readline().replace(' ', '').replace('\n', '')
		except IOError:
			raise ex.GBaseGeneralException('Could not open \"' + fileName + '\"') 

		return '<GBaseType=\"' + valueToLookFor + '\">' == readData

	def FileIsModel(self, fileName):
		return self.FileIsTable(fileName, "model")

