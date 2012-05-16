#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import GBaseCommandString
import GBaseExceptions
from os import system as s

class testGBaseCommandString(unittest.TestCase):
	def setUp(self):
		self.beingTested = GBaseCommandString.GBaseCommandString()

	def test_ImplantUnderlinesImplantsUnderlinesForAVeryLargeQueryStringWithManySpaces(self):
		queryData = 'MOD ROW WITH attr1:"this is a value" attr2:"another value" in test that attr2:"Changed value" attr3:"new value that is cool"' 
		expectedResult = 'MOD ROW WITH attr1:this_is_a_value attr2:another_value in test that attr2:Changed_value attr3:new_value_that_is_cool'
		self.assertEqual(self.beingTested.ImplantUnderlines(queryData), expectedResult)
	
	def test_ImplantUnderlinesDoesNothingWhenNoSpacesAreGiven(self):
		queryData = "DEL ROW WITH id:5 foodname:Pizza in test"
		expectedResult = "DEL ROW WITH id:5 foodname:Pizza in test"
		self.assertEqual(self.beingTested.ImplantUnderlines(queryData), expectedResult)

	def test_ImplantUnderlinesWorksWithAMixtureOfQuotesAndOthers(self):
		queryData = 'DEL ROW WITH id:5 foodname:"Pizza and bread" in test'
                expectedResult = "DEL ROW WITH id:5 foodname:Pizza_and_bread in test"
                self.assertEqual(self.beingTested.ImplantUnderlines(queryData), expectedResult)

	def test_SetActsUponSetsCorrectlyForRowTableColAndGBase(self):
		self.beingTested.SetActsUpon(['command', 'taBle', '...'])
		self.assertEqual(self.beingTested.ActsUpon, 'table')
		self.beingTested.SetActsUpon(['command', 'GBase', '...'])
                self.assertEqual(self.beingTested.ActsUpon, 'gbase')
		self.beingTested.SetActsUpon(['command', 'CoL', '...'])
                self.assertEqual(self.beingTested.ActsUpon, 'col')
		self.beingTested.SetActsUpon(['command', 'rOW', '...'])
                self.assertEqual(self.beingTested.ActsUpon, 'row')

	def test_SetActsUponThrowsAnExceptionIfThereIsNoSecondElement(self):
		self.assertRaises(GBaseExceptions.GBaseCommandMustBeFollowedByException, self.beingTested.SetActsUpon, ['command'])

	def test_GetTableNameAfterInGetsTheTableNameIfThereIsInFactAnInClause(self):
		self.assertEqual(self.beingTested.GetTableNameAfterIn(['command', '...', 'iN', 'tableName', '...']), 'tablename')

	def test_ifNoInClauseIsGivenGetTableNameAfterInRaisesAGBaseException(self):
		self.assertRaises(GBaseExceptions.GBaseCommandMustBeFollowedByException, self.beingTested.GetTableNameAfterIn, ['command', '...', 'name', '...'])

	def test_GetAttributesInClauseWorksForAProperWithStatement(self):
		pairs = self.beingTested.GetAttributesInClause(['command', 'somethingelse', 'wIth', 'attr1:value1', 'attr2:value2', 'iN', 'table'], 'with')
		self.assertEqual(pairs[0], 'attr1:value1')
		self.assertEqual(pairs[1], 'attr2:value2')		

	def test_GetAttributesInClauseWorksForAProperThatStatement(self):
                pairs = self.beingTested.GetAttributesInClause(['command', 'somethingelse', 'that', 'attr1:value1', 'attr2:value2'], 'that')
                self.assertEqual(pairs[0], 'attr1:value1')
                self.assertEqual(pairs[1], 'attr2:value2')

	def test_SetAttributesBetweenClausesWorksForAWithCommand(self):
		self.beingTested.SetAttributesBetweenClauses(['get', 'row', 'with', 'attr1:value1', 'attr2:value2', 'in', 'tableName'], 'wiTh')
		self.assertTrue(self.beingTested.WithAttributes[0] == 'attr1' and self.beingTested.WithAttributes[1] == 'attr2')
		self.assertTrue(self.beingTested.WithValues[0] == 'value1' and self.beingTested.WithValues[1] == 'value2')

	def test_SetAttributesBetweenClausesWorksForAThatCommand(self):
                self.beingTested.SetAttributesBetweenClauses(['get', 'row', '...', 'tHat', 'attr1:value1', 'attr2:value2'], 'ThAt')
                self.assertTrue(self.beingTested.ThatAttributes[0] == 'attr1' and self.beingTested.ThatAttributes[1] == 'attr2')
                self.assertTrue(self.beingTested.ThatValues[0] == 'value1' and self.beingTested.ThatValues[1] == 'value2')

	def test_GetPropertyNameWorksForAnExistingProperty(self):
		self.assertEqual(self.beingTested.GetPropertyName(['command', 'NaMe', '..'], 1), 'name')

	def test_IfBoundsOfListAreExceededThenGetPropertyNameThrowsAnException(self):
		self.assertRaises(GBaseExceptions.GBaseGeneralException, self.beingTested.GetPropertyName, ['command'], 1)

	def tearDown(self):
		pass

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseCommandString))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
