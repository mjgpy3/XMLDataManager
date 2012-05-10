#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import GBaseBrain
import GBaseExceptions

class testGBaseBrain(unittest.TestCase):
	def setUp(self):
		self.beingTested = GBaseBrain.GBaseBrain()

	def test_FileIsModelWorksForARealModel(self):
		self.assertTrue(self.beingTested.FileIsModel("./GBaseBrain_Data/testgbase.gbs"))
	
	def test_FileIsModelWorksForAFakeModel(self):
                self.assertFalse(self.beingTested.FileIsModel("./GBaseBrain_Data/testtable-testgbase.gbs"))

	def test_FileIsTableWorkForARealTable(self):
                self.assertTrue(self.beingTested.FileIsTable("./GBaseBrain_Data/testtable-testgbase.gbs"))

        def test_FileIsTableWorksForAFakeTable(self):
                self.assertFalse(self.beingTested.FileIsTable("./GBaseBrain_Data/testgbase.gbs"))

	def test_FileIsXXXXThrowsAnExceptionIfFileDNE(self):
                self.assertRaises(GBaseExceptions.GBaseGeneralException, self.beingTested.FileIsTable, ("./GBaseBrain_Data/monkeyfoo.txt"))

	def test_GetTableNameWorksReturnsCorrectFormat(self):
		self.assertEqual(self.beingTested.GetTableFileName("monkey", "testgbase"), "./monkey-testgbase.gbs")

	def test_GetModelNameWorksReturnsCorrectFormat(self):
                self.assertEqual(self.beingTested.GetModelFileName("testgbase"), "./testgbase.gbs")

	def test_HaveSameElementsWorksForSomeRandomCases(self):
                self.assertTrue(self.beingTested.HaveSameElements([], []))
		self.assertTrue(self.beingTested.HaveSameElements([4, 5, 2], [2, 5, 4]))
		self.assertFalse(self.beingTested.HaveSameElements([2, 3], [4, 3, 2]))
		self.assertTrue(self.beingTested.HaveSameElements(["foo", 42, "Python!", "Michael", 3], [3, "foo", "Python!", "Michael", 42]))
		self.assertFalse(self.beingTested.HaveSameElements([42], []))

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseBrain))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
