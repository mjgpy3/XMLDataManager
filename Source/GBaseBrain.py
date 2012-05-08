#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Tue May  8 14:46:12 EDT 2012
# 
# 

import GBaseExceptions as ex

class GBaseBrain:
	def __init__(self, location = "./", separator = "-", extension = ".gbs"):
		self.location = location
		self.separator = separator
		self.extension = extension

        """ Returns true if the passed fileName is a table """
	def FileIsTable(self, fileName, valueToLookFor = "table"):
		readData = ""
		try:
			with open(fileName, 'r') as f:
				readData = f.readline().replace(' ', '').replace('\n', '')
		except IOError:
			raise ex.GBaseGeneralException('Could not open \"' + fileName + '\"') 

		return '<GBaseType=\"' + valueToLookFor + '\">' == readData

	""" Returns true if the passed filename is a model """
	def FileIsModel(self, fileName):
		return self.FileIsTable(fileName, "model")


	""" Returns the corrisponding table's file name of the passed information """
	def GetTableFileName(self, tableName, modelName):
		return self.location + tableName.lower() + self.separator + modelName.lower() + self.extension

	""" Returns the corrisponding table's file name of the passed information """
        def GetModelFileName(self, modelName):
		return self.location + modelName.lower() + self.extension
