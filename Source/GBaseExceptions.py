#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 19:44:17 EDT 2012
# 
# 

""" Exception for when a command is not followed by something"""
class GBaseCommandMustBeFollowedByException(Exception):
	def __init__(self, commandName, followedBy):
		self.message = 'The command: \"' + commandName.upper() + '\" must be followed by ' + followedBy
	def __str__(self):
		return repr(self.message)

""" Exception for when an unrecognized command is given """
class GBaseDoesNotRecognizeCommandException(Exception):
	def __init__(self, commandName):
		self.message = '\"' + commandName + '\" is not recognized as a GBase command'
        def __str__(self):
                return repr(self.message)

""" Exception for when a ROW command does not have a WITH Clause """
class GBaseRowCommandMustHaveWithClauseException(Exception):
	def __init__(self):
		self.message = "GBase ROW command must have a WITH clause specifying attr:value pairs"
        def __str__(self):
                return repr(self.message)

""" Exception for when a MOD command does not have a THAT Clause """
class GBaseModCommandMustHaveThatClauseException(Exception):
	def __init__(self):
		self.message = "GBase MOD command must have a THAT clause specifying attr:value pairs"
	def __str__(self):
		return repr(self.message)

""" Exception for when attr:value pairs are wrong """
class GBaseAttrValuePairsMustBeOfFormException(Exception):
	def __init__(self):
		self.message = "GBase attr value pairs must be specified as [attr]:[value]"
	def __str__(self):
		return repr(self.message)

""" Exception for general problems """
class GBaseGeneralException(Exception):
	def __init__(self, message):
		self.message = message
	def __str__(self):
		return repr(self.message)
