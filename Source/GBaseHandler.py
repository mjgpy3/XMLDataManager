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
import os

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
		gbaseRoot[1].append(tableInstance)

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

		if not found or not self.brain.FileIsTable(self.brain.GetTableFileName(inThisTable, self.use)):
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
		
	def GenRowIn(self, attrs, values, inThisTable):
		attrsFromFile = []
		dictToWrite = {}
		found = False

                for tableInstance in XML.parse(self.brain.GetModelFileName(self.use)).getroot()[1]:
                        if tableInstance.attrib["Name"] == inThisTable.lower():
                                found = True
                                break

                if not found or not self.brain.FileIsTable(self.brain.GetTableFileName(inThisTable, self.use)):
                        raise GBaseExceptions.GBaseGeneralException("Table" + inThisTable + "does not exist in model")
	

		for i in range(len(attrs)):
                        attrs[i] = attrs[i].lower()

		for field in XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()[1]:
			attrsFromFile.append(field.attrib["Name"])

		if not self.brain.HaveSameElements(attrs, attrsFromFile):		
			raise GBaseExceptions.GBaseGeneralException("Some given attribute does not match the file")

		try: 
			for i in range(len(attrs)):
				dictToWrite[attrs[i]] = values[i] 		
		except:
			raise GBaseExceptions.GBaseGeneralException("Every attribute must have a value")

		entity = XML.Element("Entity")

		for i in range(len(attrs)):
			data = XML.Element("D")
			data.text = dictToWrite[attrsFromFile[i]]
			entity.append(data)

		structure = XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()

		structure[2].append(entity)

		with open(self.brain.GetTableFileName(inThisTable, self.use), "w") as outputFile:
                        XML.ElementTree(structure).write(outputFile)


	def GetRowWith(self, attrs, values, inThisTable):
		attrsFromFile = []
                dictToTest = {}
                found = False
		result = []
		nameToLocation = {}
		index = 0

                for tableInstance in XML.parse(self.brain.GetModelFileName(self.use)).getroot()[1]:
                        if tableInstance.attrib["Name"] == inThisTable.lower():
                                found = True
                                break

                if not found or not self.brain.FileIsTable(self.brain.GetTableFileName(inThisTable, self.use)):
                        raise GBaseExceptions.GBaseGeneralException("Table" + inThisTable + "does not exist in model")


                for i in range(len(attrs)):
                        attrs[i] = attrs[i].lower()

                for field in XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()[1]:
                        attrsFromFile.append(field.attrib["Name"])
			nameToLocation[field.attrib["Name"]] = index
			index += 1

                if not len(set(attrs) - set(attrsFromFile)) == 0:
                        raise GBaseExceptions.GBaseGeneralException("Some given attribute does not match the file")

                try:
                        for i in range(len(attrs)):
                                dictToTest[attrs[i]] = values[i]
                except:
                        raise GBaseExceptions.GBaseGeneralException("Every attribute must have a value")

                structure = XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()

                for entity in structure[2]:
			allMatch = True
			for attribute in attrs:
				if entity[nameToLocation[attribute]].text != dictToTest[attribute]:
					allMatch = False
					break
			if allMatch:
				toAppend = []
				for i in entity:
				 	toAppend.append(i.text)
				result.append(toAppend)
		return result

	def DelGBase(self, gbaseName):
		if self.brain.FileIsModel(self.brain.GetModelFileName(gbaseName)):
			for instance in XML.parse(self.brain.GetModelFileName(gbaseName)).getroot()[1]:
				os.remove(instance[0].text)
	
			os.remove(self.brain.GetModelFileName(gbaseName))
			
	def DelTable(self, tableName):
		structure = XML.parse(self.brain.GetModelFileName(self.use)).getroot()
		for instance in structure[1]:
			if instance.attrib["Name"] == tableName.lower():
				os.remove(instance[0].text)
				structure[1].remove(instance)
				break

		with open(self.brain.GetModelFileName(self.use), "w") as outputFile:
                        XML.ElementTree(structure).write(outputFile)

	def DelRowWith(self, attrs, values, inThisTable):
		structure = XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()
		allData = self.GetRowWith([], [], inThisTable)
		dataToRemove = self.GetRowWith(attrs, values, inThisTable)

		for toRemove in dataToRemove:
			for data in allData:
				if len(set(toRemove) - set(data)) == 0:
					allData.remove(toRemove)
					break
		
		structure[2] = XML.Element("TableData")
	
		for data in allData:
			entryTag = XML.Element("Entry")
			for datum in data:
				dTag = XML.Element("D")
				dTag.text = datum
				entryTag.append(dTag)

			structure[2].append(entryTag)
		
		with open(self.brain.GetTableFileName(inThisTable, self.use), "w") as outputFile:
                        XML.ElementTree(structure).write(outputFile)

	def DelColIn(self, colName, inThisTable):
		foundAndAt = [False, -1]
		structure = XML.parse(self.brain.GetTableFileName(inThisTable, self.use)).getroot()

		for field in structure[1]:
			foundAndAt[1] += 1
			if colName.lower() == field.attrib['Name']:
				foundAndAt[0] = True
				structure[1].remove(field)
				break

		if foundAndAt[0]:
			for entry in structure[2]:
				entry.remove(entry[foundAndAt[1]])
	
			with open(self.brain.GetTableFileName(inThisTable, self.use), "w") as outputFile:
        	                XML.ElementTree(structure).write(outputFile)

