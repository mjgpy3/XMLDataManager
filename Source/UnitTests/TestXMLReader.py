#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import XMLReader
import GBaseExceptions

class testXMLReader(unittest.TestCase):
	def setUp(self):
		self.beingTested = XMLReader.XMLReader()

	def test_getEntireFileGetsTheFileIfExists(self):
		self.assertEqual(self.beingTested.GetEntireFile("./XMLReader_Data/File.txt"), 'this is a dummy data file\nThere is some information in here...\nNothing really useful\n')
	
	def test_ifFileDoesNotExistExceptionIsThrown(self):
		self.assertRaises(GBaseExceptions.GBaseGeneralException, self.beingTested.GetEntireFile, ("FooBar.poop"))

	def test_openOpensCorrectly(self):
		self.beingTested.Open("./XMLReader_Data/File.txt")
		tempData = self.beingTested.reader.read()
                self.assertEqual(tempData, 'this is a dummy data file\nThere is some information in here...\nNothing really useful\n')

        def test_openThrowsAGBaseExceptionIfTheFileDoesNotExist(self):
                self.assertRaises(GBaseExceptions.GBaseGeneralException, self.beingTested.Open, ("FooBar.poop"))

	def test_closeThrowExceptionIfNothingHasEverBeenOpened(self):
		self.assertRaises(AttributeError, self.beingTested.Close)

	def test_closeClosesCorrectly(self):
                self.beingTested.Open("./XMLReader_Data/File.txt")
                self.assertEqual(self.beingTested.Close(), None)

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testXMLReader))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
