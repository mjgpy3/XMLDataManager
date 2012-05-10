#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import GBaseHandler
import GBaseExceptions
from os import system as s

class testGBaseHandler(unittest.TestCase):
	def setUp(self):
		self.beingTested = GBaseHandler.GBaseHandler()

	def test_GenGBaseMakesANiceFile(self):
                self.beingTested.GenGBase("foobar")
		with open("./foobar.gbs", 'r') as readToTest:
			fileData = readToTest.read()
			self.assertEqual(fileData[:51], '<GBase Type="model"><HeaderInfo><Name>foobar</Name>')
			self.assertEqual(fileData[-67:], '</CreationDate><Access>A</Access></HeaderInfo><TableData /></GBase>')
		s("rm ./foobar.gbs")

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseHandler))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
