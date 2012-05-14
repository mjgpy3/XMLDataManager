#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May 14 12:26:31 EDT 2012
# 
# 

import GBaseExceptions

class GBaseCommandString:
	def __init__(self):
		self.ResetCommand()
		
	""" Converts a string into self (the calling GBaseCommandString object) """
	def StringToCommandString(self, convertMe):
		partsOfCommand = self.ImplantUnderlines(convertMe).split(' ')
		
		if partsOfCommand[0].lower() == "use":
			self.GBaseName = self.GetPropertyName(partsOfCommand, 1) 
		elif partsOfCommand[0].lower() == "gen":
			pass
		elif partsOfCommand[0].lower() == "get":
			pass
		elif partsOfCommand[0].lower() == "del":
			pass
		elif partsOfCommand[0].lower() == "mod":
			pass
		elif partsOfCommand[0].lower() == "help":
			pass
		else:
			pass

	""" Gets a property at an expected location """
	def GetPropertyName(self, partsOfCommand, wherePropertyShouldBe):
		try:
			return partsOfCommand[wherePropertyShouldBe].lower()
		except:
			raise GBaseExceptions.GBaseGeneralException("Missing property") 

	""" Returns a string with no quotes that has underlines where spaces were in the quotes, 1 level only """
	def ImplantUnderlines(self, parseMe):
		inQuotes = False
		result = ""
		for i in range(len(parseMe)):
			if not inQuotes:
				if parseMe[i] == '"':
					inQuotes = True
				else:
					result += parseMe[i]
			elif inQuotes:
				if parseMe[i] == ' ':
					result += '_'
				elif parseMe[i] == '"':
					inQuotes = False
				else:
					result += parseMe[i]

		return result

	def ResetCommand(self):
		self.Command = ""
                self.ActsUpon = ""
                self.TableName = ""
                self.GBaseName = ""
                self.ColName = ""
                self.WithAttributes = []
                self.WithValues = []
                self.ThatAttributes = []
                self.ThatValues = []
                self.ErrorOccured = False
				
