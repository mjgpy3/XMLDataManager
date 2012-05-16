#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import GBaseExceptions as beingTested

class testGBaseExceptions(unittest.TestCase):
	def setUp(self):
		pass

	def test_ensureThatGeneralExceptionWorksAndMessageIsCorrect(self):
		try:
			raise beingTested.GBaseGeneralException("Something Failed")
			self.fail("Code shouldn't have been reached")
		except beingTested.GBaseGeneralException as e:
			self.assertEqual(e.message, "Something Failed")

	def test_ensureThatMustBeFollowedByExceptionWorksAndMessageIsCorrect(self):
                try:
                        raise beingTested.GBaseCommandMustBeFollowedByException("GEN", "Row, Col, Table or GBase")
                        self.fail("Code shouldn't have been reached")
                except beingTested.GBaseCommandMustBeFollowedByException as e:
                        self.assertEqual(e.message, 'The command: "GEN" must be followed by Row, Col, Table or GBase')

	def test_ensureThatUnrecognizedCommandExceptionWorksAndMessageIsCorrect(self):
                try:
                        raise beingTested.GBaseDoesNotRecognizeCommandException("foobar")
                        self.fail("Code shouldn't have been reached")
                except beingTested.GBaseDoesNotRecognizeCommandException as e:
                        self.assertEqual(e.message, '"foobar" is not recognized as a GBase command')


	def test_ensureThatNeedThatExceptionWorksAndMessageIsCorrect(self):
                try:
                        raise beingTested.GBaseRowCommandMustHaveClauseException("CLAUSE")
                        self.fail("Code shouldn't have been reached")
                except beingTested.GBaseRowCommandMustHaveClauseException as e:
                        self.assertEqual(e.message, 'GBase ROW command must have a CLAUSE clause specifying attr:value pairs')

	def test_ensureThatUnFormattedAttrValExceptionWorksAndMessageIsCorrect(self):
                try:
                        raise beingTested.GBaseAttrValuePairsMustBeOfFormException()
                        self.fail("Code shouldn't have been reached")
                except beingTested.GBaseAttrValuePairsMustBeOfFormException as e:
                        self.assertEqual(e.message, "GBase attr value pairs must be specified as [attr]:[value]")


def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseExceptions))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
