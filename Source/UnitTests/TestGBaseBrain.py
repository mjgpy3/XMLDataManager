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

	def test_fileIsModelWorksForARealModel(self):
		self.assertTrue(self.beingTested.FileIsModel("./GBaseBrain_Data/testgbase.gbs"))
	
	def test_fileIsModelWorksForAFakeModel(self):
                self.assertFalse(self.beingTested.FileIsModel("./GBaseBrain_Data/testtable-testgbase.gbs"))

	def test_fileIsTalbeWorkForARealTable(self):
                self.assertTrue(self.beingTested.FileIsTable("./GBaseBrain_Data/testtable-testgbase.gbs"))

        def test_fileIsTableWorksForAFakeTable(self):
                self.assertFalse(self.beingTested.FileIsTable("./GBaseBrain_Data/testgbase.gbs"))

	def test_FileIsXXXXThrowsAnExceptionIfFileDNE(self):
                self.assertRaises(GBaseExceptions.GBaseGeneralException, self.beingTested.FileIsTable, ("./GBaseBrain_Data/monkeyfoo.txt"))

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseBrain))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
