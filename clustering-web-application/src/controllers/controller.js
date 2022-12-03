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
      /*const response = await fetch(process.env.API_URL + "/GetUsersList", {
        method: 'get',
        headers: { 'Content-Type': 'application/json' }
      })
      const responseJSON = await response.json()*/
      res.render('main/index')
    } catch (error) {
      next(error)
    }
  }

  async KMeansClustering (req, res, next) {
    const response = await fetch(process.env.API_URL + "/KMeansClustering", {
        method: 'post',
        body: await JSON.stringify(req.body),
        headers: { 'Content-Type': 'application/json' }
    })
    res.setHeader('Content-Type', 'application/json');
    res.writeHead(200)
    res.end(await response.text())
  }

  async HierarchichalClustering (req, res, next) {
    const response = await fetch(process.env.API_URL + "/HierarchichalClustering", {
        method: 'post',
        body: await JSON.stringify(req.body),
        headers: { 'Content-Type': 'application/json' }
    })
    res.setHeader('Content-Type', 'application/json');
    res.writeHead(200)
    res.end(await response.text())
  }

  async respondWithJSON (res, response) {
    res.setHeader('Content-Type', 'application/json');
    res.writeHead(200)
    res.end(await response.text())
  }
}