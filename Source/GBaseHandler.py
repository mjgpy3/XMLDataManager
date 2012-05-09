#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Sun May  6 20:12:34 EDT 2012
# 
# 

import GBaseBrain
import xml.etree.ElementTree as XML
import getpass
import datetime

class GBaseHandler:
	def __init__(self):
		self.use = ""
		self.brain = GBaseBrain.GBaseBrain()

	def Use(self, gbaseName):
		if self.brain.FileIsModel(self.brain.GetModelFileName(gbaseName)):
			self.use = gbaseName.lower()

	def GenGBase(self, gbaseName):
		root = XML.Element("GBase")
		root.attrib['Type'] = "model"
		
		headerInfo = XML.Element("HeaderInfo")
		name = XML.Element("Name")
		name.text = gbaseName.lower()
		headerInfo.append(name)
		creator = XML.Element("Creator")
		creator.text = getpass.getuser()
		headerInfo.append(creator)
		creationDate = XML.Element("CreationDate")
		creationDate.Text = str(datetime.datetime.now())
		headerInfo.append(creationDate)
		access = XML.Element("Access")
		access.text = "A"
		headerInfo.append(access)
		root.append(headerInfo)
		root.append(XML.Element("TableData"))
		
		with open(self.brain.GetModelFileName(gbaseName), "w") as outputFile:
			XML.ElementTree(root).write(outputFile)

		self.Use(gbaseName)
