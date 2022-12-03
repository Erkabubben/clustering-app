/**
 * Module for the Controller.
 *
 * @author Erik Lindholm <elimk06@student.lnu.se>
 * @author Mats Loock
 * @version 1.0.0
 */

import fetch from 'node-fetch'

/**
 * Encapsulates a controller.
 */
export class Controller {
  /**
   * Displays the index page.
   *
   * @param {object} req - Express request object.
   * @param {object} res - Express response object.
   * @param {Function} next - Express next middleware function.
   */
  async index (req, res, next) {
    try {
      res.render('main/index')
    } catch (error) {
      next(error)
    }
  }

  /**
   * Makes a K-Means Clustering request to the Clustering API and returns the result JSON.
   *
   * @param {object} req - Express request object.
   * @param {object} res - Express response object.
   * @param {Function} next - Express next middleware function.
   */
  async KMeansClustering (req, res, next) {
    const response = await fetch(process.env.API_URL + '/KMeansClustering', {
      method: 'get',
      headers: { 'Content-Type': 'application/json' }
    })
    res.setHeader('Content-Type', 'application/json')
    res.writeHead(200)
    res.end(await response.text())
  }

  /**
   * Makes a Hierarchical Clustering request to the Clustering API and returns the result JSON.
   *
   * @param {object} req - Express request object.
   * @param {object} res - Express response object.
   * @param {Function} next - Express next middleware function.
   */
  async HierarchicalClustering (req, res, next) {
    const response = await fetch(process.env.API_URL + '/HierarchicalClustering', {
      method: 'get',
      headers: { 'Content-Type': 'application/json' }
    })
    res.setHeader('Content-Type', 'application/json')
    res.writeHead(200)
    res.end(await response.text())
  }
}
