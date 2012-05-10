#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Sun May  6 20:12:34 EDT 2012
# 
# 

import GBaseBrain
import xml.etree.ElementTree as XML
import getpass
import datetime
import GBaseExceptions

class GBaseHandler:
	def __init__(self):
		self.use = ""
		self.brain = GBaseBrain.GBaseBrain()

	def Use(self, gbaseName):
		if self.brain.FileIsModel(self.brain.GetModelFileName(gbaseName)):
			self.use = gbaseName.lower()
	
	def GenGBase(self, gbaseName):
		root = XML.Element("GBase", Type="model")		
		headerInfo = XML.Element("HeaderInfo")
		name = XML.Element("Name")
		name.text = gbaseName.lower()
		headerInfo.append(name)
		creator = XML.Element("Creator")
		creator.text = getpass.getuser()
		headerInfo.append(creator)
		creationDate = XML.Element("CreationDate")
		creationDate.text = str(datetime.datetime.now())
		headerInfo.append(creationDate)
		access = XML.Element("Access")
		access.text = "A"
		headerInfo.append(access)
		root.append(headerInfo)
		root.append(XML.Element("TableData"))

		with open(self.brain.GetModelFileName(gbaseName), "w") as outputFile:
			XML.ElementTree(root).write(outputFile)

		self.Use(gbaseName)

	def GenTable(self, tableName):
		tree = XML.parse(self.brain.GetModelFileName(self.use))
		gbaseRoot = tree.getroot()
		tableInstance = XML.Element("TableInstance", Name=tableName.lower())
		location = XML.Element("Location")		
		location.text = self.brain.GetTableFileName(tableName, self.use)
		tableInstance.append(location)
		gbaseRoot.append(tableInstance)

		with open(self.brain.GetModelFileName(self.use), "w") as outputFile:                
        		XML.ElementTree(gbaseRoot).write(outputFile)
		
		root = XML.Element("GBase", Type="table")
		tableHeaderInfo = XML.Element("TableHeaderInfo")
		name = XML.Element("Name")
		name.text =  tableName.lower()
		tableHeaderInfo.append(name)
		creator = XML.Element("Creator")
		creator.text = getpass.getuser()
		tableHeaderInfo.append(creator)
		root.append(tableHeaderInfo)
		tableFields = XML.Element("TableFields")
		root.append(tableFields)
		tableData = XML.Element("TableData")
		root.append(tableData)
		
		with open(self.brain.GetTableFileName(tableName, self.use), "w") as outputFile:
                        XML.ElementTree(root).write(outputFile)


	def GenColIn(self, colName, colType, inThisTable):
		found = False 
		for tableInstance in XML.parse(self.brain.GetModelFileName(self.use)).getroot()[1]:
			if tableInstance.attrib["Name"] == inThisTable.lower():
				found = True
				break
		if not found:
			raise GBaseExceptions.GBaseGeneralException("Table" + inThisTable + "does not exist in model")

		structure = XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()
		field = XML.Element("Field", Name=colName.lower())
		type = XML.Element("Type")
		type.text = colType.lower()
		field.append(type)
		structure[1].append(field)
		data = XML.Element("D")
		for entry in structure[2]:
			entry.append(data)

		with open(self.brain.GetTableFileName(inThisTable, self.use), "w") as outputFile:
                        XML.ElementTree(structure).write(outputFile)
		

a = GBaseHandler()

a.Use("testgbase")

a.GenColIn("foo", "int", "cars")
