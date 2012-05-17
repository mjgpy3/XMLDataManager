#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Thu May 17 00:47:06 EDT 2012
# 
# 

from GBaseHandler import GBaseHandler as gbh
from GBaseCommandString import GBaseCommandString as gbc

commandString = gbc()
handler = gbh()
errorOccured = False

while True:
	command = raw_input('gbase> ')

	if command.lower() == 'exit':
		exit()

	try:
		commandString.StringToCommandString(command)
	except Exception as e:
		errorOccured = True
		print e

	if not errorOccured:
		try:
			if commandString.Command == 'use':
				handler.Use(commandString.GBaseName)
			elif commandString.Command == 'gen':
				if commandString.ActsUpon == 'gbase':
					handler.GenGBase(commandString.GBaseName)
				elif commandString.ActsUpon == 'table':
					handler.GenTable(commandString.TableName)
				elif commandString.ActsUpon == 'col':
					handler.GenColIn(commandString.ColName, "string", commandString.TableName)
				elif commandString.ActsUpon == 'row':
					handler.GenRowIn(commandString.WithAttributes, commandString.WithValues, commandString.TableName)
			elif commandString.Command == 'del':
				if commandString.ActsUpon == 'gbase':
					handler.DelGBase(commandString.GBaseName)
                	        elif commandString.ActsUpon == 'table':
					handler.DelTable(commandString.TableName)
                	        elif commandString.ActsUpon == 'col':
					handler.DelColName(commandString.ColName, commandString.TableName)
				elif commandString.ActsUpon == 'row':
					handler.DelRowWith(commandString.WithAttributes, commandString.WithValues, commandString.TableName)
			elif commandString.Command == 'get':
				result = handler.GetRowWith(commandString.WithAttributes, commandString.WithValues, commandString.TableName)
				for tuple in result:
					for datum in tuple:
						print "{:<30}".format(datum),
					print ''
			elif commandString.Command == 'mod':
				handler.ModRowWith(commandString.WithAttributes, commandString.WithValues, commandString.TableName, commandString.ThatAttributes, commandString.ThatValues)
			elif commandString.Command == 'help':
				print "Help not yet implemented"
		except Exception as e:
			print e

	errorOccured = False
