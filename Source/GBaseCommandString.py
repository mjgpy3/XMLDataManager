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

		self.ResetCommand()
		
		if partsOfCommand[0].lower() == "use":
			self.GBaseName = self.GetPropertyName(partsOfCommand, 1) 
		elif partsOfCommand[0].lower() == "gen":
			self.SetDelOrGen(partsOfCommand, convertMe)
		elif partsOfCommand[0].lower() == "get":
			self.SetActsUpon(partsOfCommand)
			self.SetAttributesBetweenClauses(partsOfCommand, "with")
			self.TableName = self.GetTableNameAfterIn(partsOfCommand)
		elif partsOfCommand[0].lower() == "del":
			self.SetDelOrGen(partsOfCommand, convertMe)
		elif partsOfCommand[0].lower() == "mod":
			self.SetActsUpon(partsOfCommand)
			self.SetAttributesBetweenClauses(partsOfCommand, "with")
			self.TableName = self.GetTableNameAfterIn(partsOfCommand)
			self.SetAttributesBetweenClauses(partsOfCommand, "that")
		elif partsOfCommand[0].lower() == "help":
			pass
		else:
			raise GBaseExceptions.GBaseDoesNotRecognizeCommandException(partsOfCommand[0])

		self.Command = partsOfCommand[0].lower()

	""" Handles all commands DEL or GEN """
	def SetDelOrGen(self, partsOfCommand, commandString):
		self.SetActsUpon(partsOfCommand)

		if self.ActsUpon == 'gbase':
			self.GBaseName = self.GetPropertyName(partsOfCommand, 2)
		elif self.ActsUpon == 'table':
			self.TableName = self.GetPropertyName(partsOfCommand, 2)
		elif self.ActsUpon == 'col':
			self.ColName = self.GetPropertyName(partsOfCommand, 2)
			self.TableName = self.GetTableNameAfterIn(partsOfCommand)
		elif self.ActsUpon == 'row':
			self.SetAttributesBetweenClauses(partsOfCommand, "with")
                    	self.TableName = self.GetTableNameAfterIn(partsOfCommand)

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

	""" Sets all attributes between clauses begun with 'begin', these may be THAT or WITH """
	def SetAttributesBetweenClauses(self, partsOfCommand, begin):
		pairs = self.GetAttributesInClause(partsOfCommand, begin.lower())
		
		if pairs == None:
			raise GBaseExceptions.GBaseRowCommandMustHaveClauseException(begin)

		tempAttrs = []
		tempValues = []

		if not pairs[0] == '*':
			for pair in pairs:
				try:
					tempAttrs.append(pair.split(':')[0])
					tempValues.append(pair.split(':')[1].replace('_', ' '))
				except:
					GBaseExceptions.GBaseAttrValuePairsMustBeOfFormException()

		if begin.lower() == 'with':
			self.WithAttributes, self.WithValues = tempAttrs, tempValues
		elif begin.lower() == 'that':
			self.ThatAttributes, self.ThatValues = tempAttrs, tempValues

	""" Actually gets the attributes in a with or that clause """
	def GetAttributesInClause(self, partsOfCommand, clause):
		for i in range(len(partsOfCommand)):
			if clause.lower() == 'with' and partsOfCommand[i].lower() == 'with':
				for k in range(1, len(partsOfCommand)):
					if partsOfCommand[k].lower() == 'in':
						return partsOfCommand[i + 1:k]
			elif clause.lower() == 'that' and partsOfCommand[i].lower() == 'that':
				return partsOfCommand[i + 1:]
	
	""" Gets a table name after an in statement, raises an error if not found """
	def GetTableNameAfterIn(self, partsOfCommand):
		for i in range(len(partsOfCommand)):
			if partsOfCommand[i].lower() == 'in': return partsOfCommand[i + 1].lower()
		raise GBaseExceptions.GBaseCommandMustBeFollowedByException(partsOfCommand[0], "an IN clause specifying the tablename")

	""" Sets the ActsUpon property, which should always be in the 1st slot (right after the command) """
	def SetActsUpon(self, partsOfCommand):
		try:
			if partsOfCommand[1].lower() in ['table', 'gbase', 'row', 'col']:
				self.ActsUpon = partsOfCommand[1].lower()
			else:
				raise GBaseExceptions.GBaseCommandMustBeFollowedByException(partsOfCommand[0], "one of the following keywords: GBASE, MODEL, COL, ROW")
		except:
			raise GBaseExceptions.GBaseCommandMustBeFollowedByException(partsOfCommand[0], "one of the following keywords: GBASE, MODEL, COL, ROW")

	""" (Re-)Initializes the command's properties """
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

