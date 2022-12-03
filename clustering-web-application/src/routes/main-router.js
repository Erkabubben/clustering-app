/**
 * Routes for the GitLab OAuth application.
 *
 * @author Erik Lindholm <elimk06@student.lnu.se>
 * @author Mats Loock
 * @version 1.0.0
 */

import express from 'express'
import { Controller } from '../controllers/controller.js'

export const router = express.Router()

const controller = new Controller()

// Map HTTP verbs and route paths to controller actions.
router.get('/', controller.index)
router.post('/findTopMatchingUsers', controller.findTopMatchingUsers)
router.post('/findMovieRecommendationsForUser', controller.findMovieRecommendationsForUser)
router.post('/findMovieRecommendationsItemBased', controller.findMovieRecommendationsItemBased)
