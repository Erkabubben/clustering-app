/**
 * Routes for the Clusters application.
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
router.get('/KMeansClustering', controller.KMeansClustering)
router.get('/HierarchicalClustering', controller.HierarchicalClustering)
