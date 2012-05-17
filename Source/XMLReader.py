#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 21:21:59 EDT 2012
# 
# 

""" Does XML operations, NOT NEEDED FOR GBASE """

import GBaseExceptions
from xml.etree.ElementTree import ElementTree

class XMLReader:
	def __init__(self):
		self.reader = None
		self.tree = ElementTree()


	""" Reads an entire file """
	def GetEntireFile(self, fileName):
		try:
			return open(fileName, 'r').read()
		except IOError:
			raise GBaseExceptions.GBaseGeneralException('Could not open \"' + fileName + '\"')			

	""" Opens a file """
	def Open(self, fileName):
		try:
			self.reader = open(fileName, 'r')
		except IOError: 
			raise GBaseExceptions.GBaseGeneralException('Could not open \"' + fileName + '\"')

	""" Closes a file if necessary """
	def Close(self):
		self.reader.close()
