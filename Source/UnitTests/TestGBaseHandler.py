#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import GBaseHandler
import GBaseExceptions

class testGBaseHandler(unittest.TestCase):
	def setUp(self):
		self.beingTested = GBaseHandler.GBaseHandler()

	def test_UseWorksProperly(self):
		self.beingTested.Use("GBaseHandler_Data/TeStGBasE")
		self.assertEqual(self.beingTested.use, "gbasehandler_data/testgbase")

	
#	def test_FileIsModelWorksForAFakeModel(self):
#                self.assertFalse(self.beingTested.FileIsModel("./GBaseBrain_Data/testtable-testgbase.gbs"))


def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseHandler))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
