/**
 * The routes.
 *
 * @author Erik Lindholm <elimk06@student.lnu.se>
 * @author Mats Loock
 * @version 1.0.0
 */

import express from 'express'
import createError from 'http-errors'
import { router as mainRouter } from './main-router.js'

export const router = express.Router()

router.use('/', mainRouter)

// Catch 404 (ALWAYS keep this as the last route).
router.use('*', (req, res, next) => next(createError(404)))
