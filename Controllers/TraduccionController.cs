﻿using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/traducciones")]
[ApiController]
public class TraduccionController : ControllerBase
{
    private readonly SistemaFerremasContext _context;

    public TraduccionController(SistemaFerremasContext context)
    {
        _context = context;
    }

    /* Tablas con Traducciones - GET: api/clientes/{traduccion} */
    [HttpGet("regiones")]
    public IActionResult ObtenerRegiones()
    {
        var regiones = _context.Regions.ToList();
        return Ok(regiones);
    }

    [HttpGet("comunas/{idRegion}")]
    public async Task<IActionResult> ObtenerComunas(int idRegion)
    {
        var comunas = await _context.Comunas.Where(c => c.IdRegion == idRegion).ToListAsync();
        return Ok(comunas);
    }

    [HttpGet("generos")]
    public IActionResult ObtenerGeneros()
    {
        var generos = _context.Generos.ToList();
        return Ok(generos);
    }

    [HttpGet("estado-civil")]
    public IActionResult ObtenerEstadosCiviles()
    {
        var estados = _context.EstadoCivils.ToList();
        return Ok(estados);
    }
    /* Tablas con Traducciones */
}
