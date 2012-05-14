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

	def tearDown(self):
		pass

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseCommandString))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
