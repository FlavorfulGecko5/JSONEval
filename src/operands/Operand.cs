interface Operand {}

interface PrimitiveOperand : Operand
{
    public PrimitiveOperand Add(PrimitiveOperand b);

    public PrimitiveOperand UnaryAdd();

    public PrimitiveOperand Sub(PrimitiveOperand b);

    public PrimitiveOperand UnarySub();

    public PrimitiveOperand Mult(PrimitiveOperand b);

    public PrimitiveOperand Div(PrimitiveOperand b);

    public PrimitiveOperand Rem(PrimitiveOperand b);

    public PrimitiveOperand And(PrimitiveOperand b);
    
    public PrimitiveOperand Or(PrimitiveOperand b);

    public PrimitiveOperand Not();

    public PrimitiveOperand Equal(PrimitiveOperand b);

    public PrimitiveOperand NotEqual(PrimitiveOperand b);

    public PrimitiveOperand LessThan(PrimitiveOperand b);

    public PrimitiveOperand LessThanEqual(PrimitiveOperand b);

    public PrimitiveOperand GreaterThan(PrimitiveOperand b);

    public PrimitiveOperand GreaterThanEqual(PrimitiveOperand b);
}