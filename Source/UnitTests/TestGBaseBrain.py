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

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseBrain))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
